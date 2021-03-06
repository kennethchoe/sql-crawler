#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY [".", "."]
RUN dotnet restore "sql-crawler.sln"
WORKDIR "/src/SqlCrawler.Web"
RUN dotnet build "SqlCrawler.Web.csproj" -c Release -o /app/build
RUN dotnet publish "SqlCrawler.Web.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .

# so that we can see all error messages
ENV ASPNETCORE_ENVIRONMENT=Development

# https://github.com/dotnet/dotnet-docker/issues/1109, allow connecting to lower TLS version
RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf

# if you can install certificate on final image, you can expose https also.
# ENV ASPNETCORE_URLS=https://+:443;http://+:80

ENTRYPOINT ["dotnet", "SqlCrawler.Web.dll"]