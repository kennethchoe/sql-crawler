[CmdletBinding()]
Param(
    [string]$port = "5004",
    [switch]$skipRebuild,
    [string]$dataPath = "docker-test-data",
    [string]$gitSqlSource = "https://github.com/kennethchoe/sql-crawler-sqls.git"
)

if (-not $PSBoundParameters.ContainsKey('skipRebuild')) {
    . .\build.ps1 -target build-docker
}

$resolvedPath = (Resolve-Path $dataPath).Path

& docker run -d -v "$($resolvedPath):/app/data" -p $port`:80 --env App__SqlSourceGitRepoPath=$gitSqlSource sql-crawler