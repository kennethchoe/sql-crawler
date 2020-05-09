# sql-crawler

Connect to multiple SQL database servers and run queries to collect data.

## When Do You Need sql-crawler?

1. Your application is single-tenant database.
2. You need to poll anonymous information from multiple single-tenant production sql servers.
3. Credential to production sql servers is managed by different department.
4. Your polling sql statement needs to go through approval process.

## Demo

[https://agilesalt.net/sql-crawler](https://agilesalt.net/sql-crawler)

## Configuration

Configure your own sql server list and sql queries git repository. Then specify them on [appsettings.json](src/SqlCrawler.Web/appsettings.json), which are [overridable with environment variables](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#environment-variables).

### Sql Server List

Comma-delimited file. `*`: required

1. ServerId`*`: unique identifier of the server.
2. ServerName: user-friendly name of the server.
3. Scope: see [Scope](Scope).
4. Description
5. UserData1, UserData2: custom info that you can use in your sql as parameter (`@UserData1`) or Handlebars replacement (`{{UserData1}}`). For other properties that you can use in your sql, see [SqlServerInfoPublic](src/SqlCrawler.Backend/Core/SqlServerInfoPublic.cs).
6. ServerDriver: `mssql` (default) or `sqlite`. To support more, add class on [Drivers](src/SqlCrawler.Backend/Drivers)
7. DataSource`*`, UseIntegratedSecurity, SqlUsername, SqlPassword: connection info consumed by ServerDrivers. See [implementation of each driver](src/SqlCrawler.Backend/Drivers) to find more details.

[View Sample](src/SqlCrawler.Web/data/sql-credentials.csv)

### Sql Queries Git Repo Url

Files ending with .sql are recognized as queries.
If approval process is needed, you may configure your git repo with permission.

[View Sample](https://github.com/kennethchoe/sql-crawler-sqls)

### Scope

SQL queries may be inside subfolders in the git repository. Then the path of query file becomes the `scope` of the query.

Server List may have optional column `Scope`. ServerId must be still unique across entire list.

When you run query, it runs against servers with matching scope or below. For example, `a/b/test.sql`'s scope is `a/b`. It will run on the server with its `Scope` values `a/b` or `a/b/c`, but not on `a` nor a server with empty  `Scope` value.

For more details, check out the [demo site](https://agilesalt.net/sql-crawler).

ServerId must be still unique across different scopes. Same rule is applied to sql query file name, so if you have the same file name in different paths, sql-crawler will raise an error.

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
