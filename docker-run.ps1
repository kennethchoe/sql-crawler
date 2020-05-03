[CmdletBinding()]
Param(
    [string]$port = "5004",
    [string]$dataPath = "docker-test-data"
)

. .\build.ps1 -target build-docker

$resolvedPath = (Resolve-Path $dataPath).Path

& docker run -d -v "$($resolvedPath):/app/data" -p $port`:80 sql-crawler