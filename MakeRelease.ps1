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
Builds and signs multiple runtime variants of a .NET application using BuildAndSign.ps1,
and generates MSI installers using BuildMSI.ps1.

.DESCRIPTION
This scripts produces a MSI file per supported runtime

Each phase invokes the existing scripts:
- TripView\BuildAndSign.ps1
- TripViewerPackage\BuildMSI.ps1

.PARAMETER Configuration
The .NET build configuration to use (e.g. Release, Debug). Default is Release.

.PARAMETER DryRun
When set to true, skips actual file operations and shows the steps that would be taken.

.PARAMETER DotnetPath
Path to the dotnet CLI executable.

.PARAMETER GpgPath
Path to the GPG executable used for signing artifacts.

.PARAMETER GpgKeyId
The key ID to use for GPG signing. If not specified, the first valid signing key will be selected.

.EXAMPLE
.\BuildAll.ps1 -Configuration Release -DryRun false -GpgKeyId "1234ABCD"

.NOTES
Make sure the BuildAndSign.ps1 and BuildMSI.ps1 scripts are in their respective subdirectories
(TripView and TripViewerPackage). This script must be run from the parent directory.
#>
param (
    [string]$Configuration = "Release",
    [ValidateSet("true", "false", "yes", "no", "on", "off")]
    [string]$DryRun = "false",
    [string]$DotnetPath = "C:\Program Files\dotnet\dotnet.exe",
    [string]$GpgPath = "c:\Program Files (x86)\GnuPG\bin\gpg.exe",
    [string]$GpgKeyId = $null
)

$version = "v0"

# our supported platforms
$runtimes = @("win-x86", "win-x64", "win-arm64")

foreach ($runtime in $runtimes) {
    Write-Host "BUILD START: $runtime"

    Push-Location "TripView"

    $publishResult = & .\BuildAndSign.ps1 `
        -Runtime $runtime `
        -Configuration $Configuration `
        -SelfContained "false" `
        -SingleFile "false" `
        -ReadyToRun "false" `
        -DotnetPath $DotnetPath `
        -GpgPath $GpgPath `
        -GpgKeyId $GpgKeyId `
        -DryRun $DryRun

    Pop-Location

    if (-not $publishResult) {
        Write-Warning "BuildAndSign.ps1 failed for runtime $runtime. Skipping..."
        continue
    }

    $publishResult | Format-List

    Push-Location "TripViewerPackage"

    & .\BuildMSI.ps1 `
        -Runtime $runtime `
        -Configuration $Configuration `
        -Version $publishResult.Version `
        -DotnetPath $DotnetPath `
        -GpgPath $GpgPath `
        -GpgKeyId $GpgKeyId `
        -DryRun $DryRun

    Pop-Location

    Write-Host "*** BUILD COMPLETE: $runtime"
    $version = $publishResult.Version
}

Write-Host "*** Building Bootstrapper EXE (32-bit)..."

Push-Location "TripViewerBootstrapper"

& .\BuildBootstrapper.ps1 `
    -Runtime "win-x86" `
    -Configuration $Configuration `
    -Version $version `
    -DotnetPath $DotnetPath `
    -GpgPath $GpgPath `
    -GpgKeyId $GpgKeyId `
    -DryRun $DryRun

    Pop-Location
