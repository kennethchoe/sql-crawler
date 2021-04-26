# execute web app without vue proxying backend
npm run build --prefix src/vue
dotnet run -p ./src/SqlCrawler.Web/SqlCrawler.Web.csproj -v n --launch-profile CypressTest