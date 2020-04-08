FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.3-alpine3.10 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-alpine3.10 AS build
WORKDIR /src
COPY ["Promitor.ResourceDiscovery.Agent.csproj", ""]

COPY . .
WORKDIR "/src/."
RUN dotnet build "Promitor.ResourceDiscovery.Agent.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Promitor.ResourceDiscovery.Agent.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Promitor.ResourceDiscovery.Agent.dll"]
