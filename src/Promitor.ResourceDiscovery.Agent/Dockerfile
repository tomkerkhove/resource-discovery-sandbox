FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Promitor.ResourceDiscovery.Agent/Promitor.ResourceDiscovery.Agent.csproj", "Promitor.ResourceDiscovery.Agent/"]
RUN dotnet restore "Promitor.ResourceDiscovery.Agent/Promitor.ResourceDiscovery.Agent.csproj"
COPY . .
WORKDIR "/src/Promitor.ResourceDiscovery.Agent"
RUN dotnet build "Promitor.ResourceDiscovery.Agent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Promitor.ResourceDiscovery.Agent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Promitor.ResourceDiscovery.Agent.dll"]