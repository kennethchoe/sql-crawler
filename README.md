# sql-crawler

Connect to multiple SQL database servers and run queries to collect data.

## When Do You Need sql-crawler?

1. Your application is single-tenant database.
2. You need to poll anonymous information from multiple single-tenant production sql servers.
3. Credential to production sql servers is managed by different department.
4. Your polling sql statement needs to go through approval process.

## Demo

[https://agilesalt.net/sql-crawler](https://agilesalt.net/sql-crawler)

## How It Works

### Development

1. Dev creates docker image that accepts 2 parameters:
   1. Location of tab-delimited file that contains sql server credentials.
   2. Location of git repository for polling sql statements.
2. Dev tests image with dev sql servers list and dev sql statement repo.

### Deployment

DevOps launches the image with production sql server credentials list file and production sql statements git repo.

### Registering Poll Sql Statements with Approval Process

Configure your git repo with permission, pull request enforced.

## Commands

* `n.bat serve` : launch vue on dev mode. Vue code is hot-reloaded.
  * You must launch back-end from Visual Studio also as IIS Express profile.
* `run-webapp.ps1` : launch webapp with vue code transpiled in it, on http:5002 and https:5003.
* `build.ps1 -target publish` : create web package that can be used to run under IIS
* `build.ps1 -target build-docker` : create web package on a linux docker image called `sql-crawler`
* `docker-run.ps1` : create linux docker image and run it as http:5004.
    *  `-port 1234` : run it on http:1234.
    *  `-dataPath real-secret-path` : instead of `docker-test-data` path, use real-secret-path where you keep actual credentials of SQL servers.
    *  `-gitSqlSource https://git-repo-url` : specify sql source. Default value is `https://github.com/kennethchoe/sql-crawler-sqls.git`, which is sample repo that shows what is possible.
    *  `-gitUsername username -gitPassword password`  : specify git repo's credential, if needed.
    *  `-skipRebuild` skips rebuilding docker image.

## Useful Docker Commands

Because I forget them after a while...

* `docker ps` : display running containers
* `docker container prune` : delete all stopped containers
* `docker logs -f container-identifier` : show console log until you ctrl+C.
* `docker inspect container-identifier` : show full info about running container, including port, volume mapping, etc.
* `docker exec -it container-identifier bash` : launch interactive bash on running container. `exit` to disconnect
* `docker exec container-identifier ls /app` : tap into running container and display the result of `ls /app`

## Backlog

* user can view result by sql server
