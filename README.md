# sql-crawler

Connect to multiple SQL database servers and run queries to collect data.

## When Do You Need sql-crawler?

1. Your application is single-tenant database.
2. You need to poll anonymous information from multiple single-tenant production sql servers.
3. You do not have credential to production sql servers.
4. Your polling sql statement needs to go through approval process.

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

* `n.bat serve`: launch vue on dev mode. Vue code is hot-reloaded.
  * You much launch back-end from Visual Studio also as IIS Express profile.
* `run-webapp.ps1`: launch webapp with vue code transpiled in it, on http:5002 and https:5003.
* `build.ps1 -target publish`: create web package that can be used to run under IIS
* `build.ps1 -target build-docker`: create web package on a linux docker image called `sql-crawler`
* `docker-run.ps1 {port#}`: create linux docker image and run it as http on the given port number. When omitted, port# is 5004.

## Backlog

* container takes sql server list input
* container takes sql source input
* user can run sql
* user can view result by sql source
* user can view result by sql server
* user can stop running sql
* user can revise sql source and sql crawler keeps the snapshot
* user can view previous session's result
* user can add custom tokens on sql server list and consume on sql source

### Done

* dev mode works with vue npm server and asp.net core server
* ci creates docker package that has vue transpiled into asp.net core server
