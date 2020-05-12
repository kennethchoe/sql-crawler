[CmdletBinding()]
Param(
    [switch]$skipRebuild
)

cd $PSScriptRoot

if (-not $PSBoundParameters.ContainsKey('skipRebuild')) {
    . .\build.ps1 -target build-docker
    if ($LASTEXITCODE -gt 0) { exit $LASTEXITCODE; }
}

$resolvedPath = (Resolve-Path "docker-test-data").Path

& docker run -d -v "$($resolvedPath):/app/data" -p $port`:80 `
    sql-crawler