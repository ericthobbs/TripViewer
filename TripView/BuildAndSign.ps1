param (
    [string]$Runtime = "win-x64",
    [bool]$SelfContained = $true,
    [bool]$SingleFile = $true,
    [bool]$ReadyToRun = $true,
    [string]$ProjectPath = ".",
    [string]$Configuration = "Release",
	[string]$DotnetPath = "C:\Program Files\dotnet\dotnet.exe",
	[string]$GitPath = "c:\Program Files\git\bin\git.exe",
	[string]$GpgPath = "c:\Program Files (x86)\GnuPG\bin\gpg.exe"
)

function Get-AssemblyVersionFromSdkProj {
    param ($projectFile)
    $versionLine = Select-String -Path $projectFile -Pattern "<AssemblyVersion>(.*?)</AssemblyVersion>" | Select-Object -First 1
    if ($versionLine -and $versionLine.Matches[0].Groups[1].Value) {
        return $versionLine.Matches[0].Groups[1].Value
    }
    return "vUnknown"
}

function Get-GitShortHash {
    & $GitPath  rev-parse --short HEAD
}

$singleFileLabel = if ($SingleFile) { "SingleFile" } else { "MultiFile" }
$readyToRunLabel = if ($ReadyToRun) { "ReadyToRun" } else { "NoRTR" }

$outputDir = Join-Path -Path "publish" -ChildPath "$singleFileLabel\$readyToRunLabel\$Runtime"
$zipOutputDir = Join-Path -Path $PWD -ChildPath $outputDir

Write-Host "Cleaning previous output..."
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $outputDir

$scFlag = if ($SelfContained) { "true" } else { "false" }
$pubFlags = @(
    "-c", $Configuration,
    "-r", $Runtime,
    "-o", $outputDir,
    "-p:PublishSingleFile=$SingleFile",
    "-p:PublishReadyToRun=$ReadyToRun",
    "-p:SelfContained=$scFlag"
)

Write-Host "Publishing for $Runtime..."
& $DotnetPath publish $ProjectPath @pubFlags

# Gather metadata
$csproj = Get-ChildItem $ProjectPath -Recurse -Filter *.csproj | Select-Object -First 1
$version = Get-AssemblyVersionFromSdkProj $csproj.FullName
$shortHash = Get-GitShortHash
$dateStr = (Get-Date -Format "yyyyMMdd")
$zipFileName = "${Runtime}_${dateStr}_v${version}_${shortHash}.zip"
$zipFullPath = Join-Path -Path "publish" -ChildPath $zipFileName

# Zip it
Write-Host "Zipping $zipFullPath..."
Compress-Archive -Path "$outputDir" -DestinationPath $zipFullPath -Force

# GPG sign
$gpgSig = "$zipFullPath.sig"
Write-Host "Signing $zipFileName with GPG..."
& $GpgPath --output $gpgSig --detach-sign $zipFullPath

#Generate SHA256 hash
Write-Host "Generating SHA256 hash for $zipFullPath"
Get-FileHash -Algorithm SHA256 $zipFullPath | ForEach-Object { $_.Hash } > "$zipFullPath.sha256"

Write-Host "`nDone"
Write-Host "Output: $zipFullPath"
Write-Host "Signature: $gpgSig"
