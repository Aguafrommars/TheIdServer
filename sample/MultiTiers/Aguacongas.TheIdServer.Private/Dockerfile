FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/Aguacongas.TheIdServer/Aguacongas.TheIdServer.csproj", "src/Aguacongas.TheIdServer/"]
RUN dotnet restore "src/Aguacongas.TheIdServer/Aguacongas.TheIdServer.csproj"
COPY . .
WORKDIR "/src/src/Aguacongas.TheIdServer"
RUN dotnet build "Aguacongas.TheIdServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aguacongas.TheIdServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aguacongas.TheIdServer.dll"]