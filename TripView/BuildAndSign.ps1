<#
MIT License

Copyright (c) 2025 Eric Hobbs

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
#>

<#
.SYNOPSIS
Publishes a .NET Core application (e.g. TripView) for the specified runtime, optionally self-contained, single-file, and ready-to-run.
Also generates a ZIP archive of the build and GPG signs the build with the default key (or one that you have specified). You may be 
prompted by your gpg agent if the key was not recently used since we are not storing the password here. if your key is passwordless, then
it should never prompt - do NOT do that.

.DESCRIPTION
This script builds and packages a .NET Core project with support for the specified runtime, output configuration, 
and optional GPG signing. It generates a zip archive which includes metadata such as Git commit hash assembly version.
the ASCII armored .sig file and SHA256 hash files are created in the same location as the zip file, which is in the "publish" folder

.PARAMETER Runtime
Target runtime identifier (RID) for publishing.
Default: "win-x64"
Examples: "linux-x64", "win-x86", "osx-arm64"
NOTE: Only win-x is supported by TripView at this time.

.PARAMETER SelfContained
Specifies whether the build should be self-contained.
Default: $true

.PARAMETER SingleFile
Publishes as a single executable file if $true.
Default: $true

.PARAMETER ReadyToRun
Enables ReadyToRun (R2R) compilation if $true.
Default: $true

.PARAMETER ProjectPath
Path to the project (.csproj) or folder containing the project.
Default: "."

.PARAMETER Configuration
Build configuration (e.g., "Release" or "Debug").
Default: "Release"

.PARAMETER DotnetPath
Full path to the dotnet CLI executable.
Default: "C:\Program Files\dotnet\dotnet.exe"

.PARAMETER GitPath
Full path to the Git executable (used for getting the current commit hash).
Default: "C:\Program Files\Git\bin\git.exe"

.PARAMETER GpgPath
Full path to the GPG executable (used for signing the ZIP archive).
Default: "C:\Program Files (x86)\GnuPG\bin\gpg.exe"

.PARAMETER GpgKeyId
GPG Key Id to use to sign with. It will attempt to autodetect the correct key (first non expired, disabled,
or revoked) that can sign to use if none is specified.
Default: ""

.PARAMETER DryRun
set to true to show what would happen if actually run.

.EXAMPLE
.\publish-app.ps1 -Runtime win-x64 -SelfContained $false -SingleFile $true -ReadyToRun $true

.EXAMPLE
.\publish-app.ps1 -Runtime linux-x64 -DotnetPath "D:\tools\dotnet\dotnet.exe" -GpgPath "D:\gnupg\gpg.exe"

.NOTES
Make sure Git and GPG are installed and available at the specified paths.
This script must be run from a Git-enabled repository to gather version and commit metadata.
If GIT or GPG are not found, they will be skipped.

#>

param (
    [ValidateSet("win-x64", "win-x86", "win-arm64")]
	[string]$Runtime = "win-x64",

    [ValidateSet("true", "false", "yes", "no", "on", "off")]
	[string]$SelfContained = $true,

    [ValidateSet("true", "false", "yes", "no", "on", "off")]
	[string]$SingleFile = $true,

    [ValidateSet("true", "false", "yes", "no", "on", "off")]
	[string]$ReadyToRun = $true,

	[string]$ProjectPath = ".",

	[string]$Configuration = "Release",

	[string]$DotnetPath = "C:\Program Files\dotnet\dotnet.exe",

	[string]$GitPath = "c:\Program Files\git\bin\git.exe",

	[string]$GpgPath = "c:\Program Files (x86)\GnuPG\bin\gpg.exe",

    [string]$GpgKeyId = $null,

    [ValidateSet("true", "false", "yes", "no", "on", "off")]
    [string]$DryRun = "false",

    [ValidateSet("true", "false", "yes", "no", "on", "off")]
    [string]$NoDeploy = "false"
)

# Converts a string to bool -- powershell seems to have issues reading arguments as types other then strings from the command line
function To-Bool($value) {
    switch ($value.ToLowerInvariant()) {
        "true"  { return $true }
        "false" { return $false }
        "yes"   { return $true }
        "no"    { return $false }
        "on"    { return $true }
        "off"   { return $false }
        default { return $false }
    }
}

function Get-AssemblyVersionFromSdkProj {
    param ($projectFile)
    $versionLine = Select-String -Path $projectFile -Pattern "<AssemblyVersion>(.*?)</AssemblyVersion>" | Select-Object -First 1
    if ($versionLine -and $versionLine.Matches[0].Groups[1].Value) {
        return $versionLine.Matches[0].Groups[1].Value
    }
    return "Unknown"
}

function Get-GitShortHash {
    if(Test-Path $GitPath) {
    & $GitPath  rev-parse --short HEAD
    }
}

$SelfContained = To-Bool $SelfContained
$ReadyToRun = To-Bool $ReadyToRun

# If GPG is in our path and the GpgKeyId is not set, we want to fetch the default key for our records.
if (Test-Path $GpgPath) 
{
    #ChatGPT magic here (I wasn't going to try and figure out how to do this without help.)
    #I did not learn a single thing here. Except that https://github.com/gpg/gnupg/blob/master/doc/DETAILS describes the contents of the fields
    if (-not $GpgKeyId) 
    {
        $now = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
        $GpgKeyId = & $GpgPath --list-secret-keys --with-colons |
        Where-Object { $_ -like 'sec:*' } |
        ForEach-Object {
            $fields = $_ -split ':'
            $keyId = $fields[4]
            $expires = $fields[6]
            $capabilities = $fields[11]

            # Check expiration
            $validExpiry = $true
            if ($expires -and $expires -ne '0') {
                try 
                {
                    $validExpiry = ([int64]$expires -gt $now)
                } 
                catch 
                {
                    $validExpiry = $false
                }
            }

            # Include if not expired and has sign capability
            if ($validExpiry -and $capabilities -match 'S') 
            {
                return $keyId
            }
        } | Select-Object -First 1
    }
}

$singleFileLabel = if (To-Bool $SingleFile) { "SingleFile" } else { "MultiFile" }
$readyToRunLabel = if (To-Bool $ReadyToRun) { "ReadyToRun" } else { "NoRTR" }
$selfContainedLabel = if (To-Bool $SelfContained) { "SC" } else { "NotSC" }

$outputDir = Join-Path -Path "publish" -ChildPath "$singleFileLabel-$selfContainedLabel\$readyToRunLabel\$Runtime"

if(To-Bool $DryRun) {
Write-Host "DRYRUN: (not) Cleaning previous output... $outputDir"
}
else {
    Write-Host "Cleaning previous output... $outputDir"
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $outputDir    
}

$publishArguments = @(
    "publish",
    "`"$ProjectPath`"",
    "--configuration", $Configuration,
    "--runtime", $Runtime,
    "--output", "`"$outputDir`"",
    "/p:PublishSingleFile=$SingleFile",
    "/p:PublishReadyToRun=$ReadyToRun",
    "/p:SelfContained=$SelfContained"
    )

if(To-Bool $DryRun) {
    Write-Host "DRYRUN: $DotnetPath $($publishArguments -join ' ')"
} else {
    Write-Host "Publishing for $Runtime to $outputDir..."
    & $DotnetPath @publishArguments    
}

if(To-Bool $NoDeploy) {
    Write-Host "Published to $outputDir";
    return;
}

# Gather metadata
$csproj = Get-ChildItem $ProjectPath -Recurse -Filter *.csproj | Select-Object -First 1
$version = Get-AssemblyVersionFromSdkProj $csproj.FullName
$shortHash = Get-GitShortHash
$dateStr = (Get-Date -Format "yyyyMMdd")
$zipFileName = "${Runtime}_${dateStr}_v${version}_${shortHash}.zip"
$zipFullPath = Join-Path -Path "publish" -ChildPath $zipFileName

# Compress the output folder into a single zip archive
if(To-Bool $DryRun) {
    Write-Host "DRYRUN: Zipping $zipFullPath..."
} else {
    Write-Host "Zipping $zipFullPath..."
    Compress-Archive -Path "$outputDir" -DestinationPath $zipFullPath -Force
}

# GPG sign the zip archive for data validation
$gpgSig = $null
if (Test-Path $GpgPath) {
    $gpgSig = "$zipFullPath.sig"
    $gpgArgs = @(
        if ($GpgKeyId) { "--default-key"; $GpgKeyId }
        "--armor"
        "--output"; $gpgSig
        "--detach-sign"; $zipFullPath
        )
    if(To-Bool $DryRun) {
        Write-Host "DRYRUN: $GpgPath $($gpgArgs -join ' ')"
    } else {
        Write-Host "Signing $zipFileName with GPG Key = `"$GpgKeyId`"..."
        & $GpgPath @gpgArgs
    }
}

# Generate SHA256 hash of the zip archive
$sha256 = "$zipFullPath.sha256"
if (To-Bool $DryRun) {
    Write-Host "DRYRUN: Generating SHA256 hash for $zipFullPath"
} else {
    Write-Host "Generating SHA256 hash for $zipFullPath"
    Get-FileHash -Algorithm SHA256 $zipFullPath | ForEach-Object { $_.Hash } > $sha256
}

#Return this object (so it can be used in other parts of the pipeline, if needed.)
[PSCustomObject]@{
    ZipFile     = "$zipFullPath"
    PublishPath = "$outputDir"
    Signature   = "$gpgSig"
    SHA256      = "$sha256"
    Version     = "$version"
    GitCommitId = "$shortHash"
    Project     = "$csproj"
    Date        = "$dateStr"
    GPGKey      = "$GpgKeyId" 
}
