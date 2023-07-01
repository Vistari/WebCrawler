FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebCrawler.Console/WebCrawler.Console.csproj", "WebCrawler.Console/"]
RUN dotnet restore "WebCrawler.Console/WebCrawler.Console.csproj"
COPY . .
WORKDIR "/src/WebCrawler.Console"
RUN dotnet build "WebCrawler.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebCrawler.Console.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "WebCrawler.Console.dll"]
