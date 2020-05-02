# execute web app without vue proxying backend
& .\n.bat build
& dotnet.exe run -p .\src\SqlCrawler.Web\SqlCrawler.Web.csproj -v n --launch-profile CypressTest