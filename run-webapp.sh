# execute web app without vue proxying backend
npm install --prefix src/vue
npm run build --prefix src/vue
dotnet dev-certs https
dotnet run -p ./src/SqlCrawler.Web/SqlCrawler.Web.csproj -v n --launch-profile CypressTest