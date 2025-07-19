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
Builds an WiX Bootstrapper using dotnet and signs it with GPG.

.DESCRIPTION
This script compiles a .wixproj-based BA EXE installer using dotnet build and then
generates detached GPG signatures and SHA256 hash files for verification.

This script is designed to be called after a successful publish step using BuildAndSign.ps1.

.PARAMETER Runtime
Target runtime identifier. Determines the platform to build for (win-x64, win-x86, win-arm64).

.PARAMETER Configuration
The .NET build configuration (e.g., Release, Debug). Default is Release.

.PARAMETER Version
The version string to use for output paths. Should typically match the assembly or product version.

.PARAMETER ProjectPath
Path to the .wixproj file. Default is "." which assumes a single .wixproj in the folder.

.PARAMETER DotnetPath
Path to the dotnet CLI executable.

.PARAMETER GpgPath
Path to the GPG executable used for signing the EXE.

.PARAMETER GpgKeyId
Optional. If specified, this key is used for signing. If not provided, the first valid key is used.

.PARAMETER DryRun
If true, shows what would happen but performs no actual build or signing operations.

.OUTPUTS
A PowerShell object with the output EXE path, GPG signature path, SHA256 hash path, and metadata.

.EXAMPLE
.\BuildBootstrapper.ps1 -Runtime win-x64 -Configuration Release -Version 1.2.3

.NOTES
Requires:
- WiX Toolset v6
- .NET SDK
- GPG for signing
- BuildAndSign.ps1 to be run prior to this to produce publish artifacts
#>
param(
    [ValidateSet("win-x64", "win-x86", "win-arm64")]
	[string]$Runtime = "win-x86",
    [string]$Configuration = "Release",
    [string]$Version = "0.0.0.0",
    [string]$ProjectPath = ".",
    [string]$DotnetPath = "C:\Program Files\dotnet\dotnet.exe",
    [string]$GpgPath = "c:\Program Files (x86)\GnuPG\bin\gpg.exe",
    [string]$GpgKeyId = $null,
    [ValidateSet("true", "false", "yes", "no", "on", "off")]
    [string]$DryRun = "false"
)

function Convert-RuntimeToPlatform {
    param([string]$runtime)

    switch ($runtime.ToLower()) {
        "win-x64"   { return "x64" }
        "win-x86"   { return "x86" }
        "win-arm64" { return "arm64" }
    }
}

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

$platform = Convert-RuntimeToPlatform -runtime $Runtime

$outputDir = Join-Path -Path "publish" -ChildPath "$Runtime-$Configuration-$Version"

Write-Host "Building EXE for platform: $platform, configuration: $Configuration"

$outputExe = "TripViewerSetup-$Version.exe"

$buildArguments = @(
    "build",
    "`"$ProjectPath`"",
    "--configuration", $Configuration,
    "--version-suffix", $Version
    "/p:Platform=$platform",
    "-a", $platform,
    "--output", "`"$outputDir`""
)

& $DotnetPath @buildArguments

Move-Item -Path $outputDir\TripViewerBootstrapper.exe $outputDir\$outputExe

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

$fullpath = $outputDir + "\$outputExe"

# GPG sign the zip archive for data validation
$gpgSig = $null
if (Test-Path $GpgPath) {
    $gpgSig = "$fullpath.sig"
    $gpgArgs = @(
        if ($GpgKeyId) { "--default-key"; $GpgKeyId }
        "--armor"
        "--output"; $gpgSig
        "--detach-sign"; $fullpath
        )
    if(To-Bool $DryRun) {
        Write-Host "DRYRUN: $GpgPath $($gpgArgs -join ' ')"
    } else {
        Write-Host "Signing $fullpath with GPG Key = `"$GpgKeyId`"..."
        & $GpgPath @gpgArgs
    }
}

# Generate SHA256 hash of the zip archive
$sha256 = "$fullpath.sha256"

Get-FileHash -Algorithm SHA256 $fullpath | ForEach-Object { $_.Hash } > $sha256

#Return this object (so it can be used in other parts of the pipeline, if needed.)
[PSCustomObject]@{
    OutputExe = "$fullpath"
    GPGKey    = "$GpgKeyId" 
    Signature = "$gpgSig"
    SHA256    = "$sha256"
    Version   = "$version"
}