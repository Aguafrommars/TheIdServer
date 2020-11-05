#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:5.0-buster AS build
WORKDIR /src
COPY ["src/Aguacongas.TheIdServer/Aguacongas.TheIdServer.csproj", "src/Aguacongas.TheIdServer/"]
COPY ["src/Aguacongas.TheIdServer.Identity/Aguacongas.TheIdServer.Identity.csproj", "src/Aguacongas.TheIdServer.Identity/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Http.Store/Aguacongas.IdentityServer.Http.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Http.Store/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Admin.Http.Store/Aguacongas.IdentityServer.Admin.Http.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Admin.Http.Store/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Store/Aguacongas.IdentityServer.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Store/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer/Aguacongas.IdentityServer.csproj", "src/IdentityServer/Aguacongas.IdentityServer/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.EntityFramework.Store/Aguacongas.IdentityServer.EntityFramework.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.EntityFramework.Store/"]
COPY ["src/Aguacongas.TheIdServer.BlazorApp/Aguacongas.TheIdServer.BlazorApp.csproj", "src/Aguacongas.TheIdServer.BlazorApp/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Admin/Aguacongas.IdentityServer.Admin.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Admin/"]
COPY ["src/Aguacongas.TheIdServer.Authentication/Aguacongas.TheIdServer.Authentication.csproj", "src/Aguacongas.TheIdServer.Authentication/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.MySql/Aguacongas.TheIdServer.Migrations.MySql.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.MySql/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.Oracle/Aguacongas.TheIdServer.Migrations.Oracle.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.Oracle/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.PostgreSQL/Aguacongas.TheIdServer.Migrations.PostgreSQL.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.PostgreSQL/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.Sqlite/Aguacongas.TheIdServer.Migrations.Sqlite.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.Sqlite/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.SqlServer/Aguacongas.TheIdServer.Migrations.SqlServer.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.SqlServer/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages/Aguacongas.TheIdServer.BlazorApp.Pages.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.KeysRotation/Aguacongas.IdentityServer.KeysRotation.csproj", "src/IdentityServer/Aguacongas.IdentityServer.KeysRotation/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Components/Aguacongas.TheIdServer.BlazorApp.Components.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Components/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Infrastructure/Aguacongas.TheIdServer.BlazorApp.Infrastructure.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Infrastructure/"]
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