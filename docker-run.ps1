$port=$args[0]
if (-not $port) { $port = 5004 }

#. .\build.ps1 -target build-docker
& docker run -d -p $port`:80 sql-crawler