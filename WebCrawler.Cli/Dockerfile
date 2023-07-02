FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebCrawler.Cli/WebCrawler.Cli.csproj", "WebCrawler.Cli/"]
RUN dotnet restore "WebCrawler.Cli/WebCrawler.Cli.csproj"
COPY . .
WORKDIR "/src/WebCrawler.Cli"
RUN dotnet build "WebCrawler.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebCrawler.Cli.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebCrawler.Cli.dll"]
