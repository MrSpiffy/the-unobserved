param(
    [string]$UnityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.12f1\Editor\Unity.exe',
    [switch]$Verify
)
$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if (-not (Test-Path -LiteralPath $UnityPath)) { throw "Unity editor not found: $UnityPath" }
$buildLog = Join-Path $projectRoot 'build.log'
$editorArgs = @('-batchmode', '-nographics', '-quit', '-projectPath', ('"{0}"' -f $projectRoot), '-executeMethod', 'ProjectSetup.Build', '-logFile', ('"{0}"' -f $buildLog))
$build = Start-Process -FilePath $UnityPath -ArgumentList $editorArgs -WindowStyle Hidden -Wait -PassThru
if ($build.ExitCode -ne 0 -or -not (Select-String -LiteralPath $buildLog -Pattern 'UNOBSERVED_BUILD_SUCCESS' -Quiet)) { throw "Build failed. See $buildLog" }
Write-Output 'Windows build succeeded.'
if ($Verify) {
    $player = Join-Path $projectRoot 'Builds\Windows\The Unobserved.exe'
    $runtimeLog = Join-Path $projectRoot 'runtime.log'
    $run = Start-Process -FilePath $player -ArgumentList @('-batchmode', '-smokeTest', '-logFile', ('"{0}"' -f $runtimeLog)) -WindowStyle Hidden -Wait -PassThru
    if ($run.ExitCode -ne 0 -or -not (Select-String -LiteralPath $runtimeLog -Pattern 'UNOBSERVED_V2_RESULT: \d+ passed; 0 failed' -Quiet)) { throw "Verification failed. See $runtimeLog" }
    Get-Content -LiteralPath (Join-Path $projectRoot 'Builds\VerificationV2\results.txt')
}
