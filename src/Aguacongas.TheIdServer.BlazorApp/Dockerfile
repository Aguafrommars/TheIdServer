#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:5.0-buster AS build
WORKDIR /src
COPY ["src/Aguacongas.TheIdServer.BlazorApp/Aguacongas.TheIdServer.BlazorApp.csproj", "src/Aguacongas.TheIdServer.BlazorApp/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Admin.Http.Store/Aguacongas.IdentityServer.Admin.Http.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Admin.Http.Store/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Store/Aguacongas.IdentityServer.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Store/"]
COPY ["src/Aguacongas.TheIdServer.BlazorApp/nginx.conf", "src/Aguacongas.TheIdServer.BlazorApp/"]
RUN dotnet restore "src/Aguacongas.TheIdServer.BlazorApp/Aguacongas.TheIdServer.BlazorApp.csproj"
COPY . .
WORKDIR "/src/src/Aguacongas.TheIdServer.BlazorApp"
RUN dotnet build "Aguacongas.TheIdServer.BlazorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aguacongas.TheIdServer.BlazorApp.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY --from=build /src/src/Aguacongas.TheIdServer.BlazorApp/nginx.conf /etc/nginx/nginx.conf
