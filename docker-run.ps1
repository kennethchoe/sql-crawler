[CmdletBinding()]
Param(
    [string]$port = "5004",
    [switch]$skipRebuild,
    [string]$dataPath = "docker-test-data",
    [string]$gitSqlSource = "https://github.com/kennethchoe/sql-crawler-sqls.git",
	[string]$gitUsername,
	[string]$gitPassword
)

cd $PSScriptRoot

if (-not $PSBoundParameters.ContainsKey('skipRebuild')) {
    . .\build.ps1 -target build-docker
    if ($LASTEXITCODE -gt 0) { exit $LASTEXITCODE; }
}

$resolvedPath = (Resolve-Path $dataPath).Path

& docker run -d -v "$($resolvedPath):/app/data" -p $port`:80 --env App__SqlSourceGitRepoUrl=$gitSqlSource  --env App__SqlSourceGitUsername=$gitUsername  --env App__SqlSourceGitPassword=$gitPassword sql-crawler