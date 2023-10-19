#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Aguacongas.TheIdServer.Duende/Aguacongas.TheIdServer.Duende.csproj", "src/Aguacongas.TheIdServer.Duende/"]
COPY ["src/Aguacongas.TheIdServer.BlazorApp/Aguacongas.TheIdServer.BlazorApp.csproj", "src/Aguacongas.TheIdServer.BlazorApp/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.ApiScopes/Aguacongas.TheIdServer.BlazorApp.Pages.ApiScopes.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.ApiScopes/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Components/Aguacongas.TheIdServer.BlazorApp.Components.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Components/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Infrastructure/Aguacongas.TheIdServer.BlazorApp.Infrastructure.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Infrastructure/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Admin.Http.Store/Aguacongas.IdentityServer.Admin.Http.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Admin.Http.Store/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Store/Aguacongas.IdentityServer.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Store/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.ApiScope/Aguacongas.TheIdServer.BlazorApp.Pages.ApiScope.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.ApiScope/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Apis/Aguacongas.TheIdServer.BlazorApp.Pages.Apis.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Apis/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Api/Aguacongas.TheIdServer.BlazorApp.Pages.Api.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Api/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Clients/Aguacongas.TheIdServer.BlazorApp.Pages.Clients.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Clients/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Client/Aguacongas.TheIdServer.BlazorApp.Pages.Client.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Client/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Cultures/Aguacongas.TheIdServer.BlazorApp.Pages.Cultures.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Cultures/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Culture/Aguacongas.TheIdServer.BlazorApp.Pages.Culture.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Culture/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProviders/Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProviders.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProviders/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider/Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Identities/Aguacongas.TheIdServer.BlazorApp.Pages.Identities.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Identities/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Identity/Aguacongas.TheIdServer.BlazorApp.Pages.Identity.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Identity/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Import/Aguacongas.TheIdServer.BlazorApp.Pages.Import.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Import/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Keys/Aguacongas.TheIdServer.BlazorApp.Pages.Keys.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Keys/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParties/Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParties.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParties/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParty/Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParty.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParty/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Roles/Aguacongas.TheIdServer.BlazorApp.Pages.Roles.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Roles/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Role/Aguacongas.TheIdServer.BlazorApp.Pages.Role.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Role/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Settings/Aguacongas.TheIdServer.BlazorApp.Pages.Settings.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Settings/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Users/Aguacongas.TheIdServer.BlazorApp.Pages.Users.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.Users/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.User/Aguacongas.TheIdServer.BlazorApp.Pages.User.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages.User/"]
COPY ["src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages/Aguacongas.TheIdServer.BlazorApp.Pages.csproj", "src/BlazorApp/Aguacongas.TheIdServer.BlazorApp.Pages/"]
COPY ["src/Aguacongas.TheIdServer/Aguacongas.TheIdServer.csproj", "src/Aguacongas.TheIdServer/"]
COPY ["src/Aguacongas.TheIdServer.Authentication/Aguacongas.TheIdServer.Authentication.csproj", "src/Aguacongas.TheIdServer.Authentication/"]
COPY ["src/Aguacongas.TheIdServer.Identity/Aguacongas.TheIdServer.Identity.csproj", "src/Aguacongas.TheIdServer.Identity/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Admin/Aguacongas.IdentityServer.Admin.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Admin/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.KeysRotation/Aguacongas.IdentityServer.KeysRotation.csproj", "src/IdentityServer/Aguacongas.IdentityServer.KeysRotation/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer/Aguacongas.IdentityServer.csproj", "src/IdentityServer/Aguacongas.IdentityServer/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.EntityFramework.Store/Aguacongas.IdentityServer.EntityFramework.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.EntityFramework.Store/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.Http.Store/Aguacongas.IdentityServer.Http.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.Http.Store/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.MongoDb.Store/Aguacongas.IdentityServer.MongoDb.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.MongoDb.Store/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.RavenDb.Store/Aguacongas.IdentityServer.RavenDb.Store.csproj", "src/IdentityServer/Aguacongas.IdentityServer.RavenDb.Store/"]
COPY ["src/IdentityServer/Aguacongas.IdentityServer.WsFederation/Aguacongas.IdentityServer.WsFederation.csproj", "src/IdentityServer/Aguacongas.IdentityServer.WsFederation/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.MySql/Aguacongas.TheIdServer.Migrations.MySql.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.MySql/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.Oracle/Aguacongas.TheIdServer.Migrations.Oracle.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.Oracle/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.PostgreSQL/Aguacongas.TheIdServer.Migrations.PostgreSQL.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.PostgreSQL/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.Sqlite/Aguacongas.TheIdServer.Migrations.Sqlite.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.Sqlite/"]
COPY ["src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.SqlServer/Aguacongas.TheIdServer.Migrations.SqlServer.csproj", "src/IdentityServer/Migrations/Aguacongas.TheIdServer.Migrations.SqlServer/"]
COPY ["src/IdentityServer/Duende/Aguacongas.IdentityServer.Admin.Duende/Aguacongas.IdentityServer.Admin.Duende.csproj", "src/IdentityServer/Duende/Aguacongas.IdentityServer.Admin.Duende/"]
COPY ["src/IdentityServer/Duende/Aguacongas.IdentityServer.Duende/Aguacongas.IdentityServer.Duende.csproj", "src/IdentityServer/Duende/Aguacongas.IdentityServer.Duende/"]
COPY ["src/IdentityServer/Duende/Aguacongas.IdentityServer.KeysRotation.Duende/Aguacongas.IdentityServer.KeysRotation.Duende.csproj", "src/IdentityServer/Duende/Aguacongas.IdentityServer.KeysRotation.Duende/"]
COPY ["src/IdentityServer/Duende/Aguacongas.IdentityServer.EntityFramework.Store.Duende/Aguacongas.IdentityServer.EntityFramework.Store.Duende.csproj", "src/IdentityServer/Duende/Aguacongas.IdentityServer.EntityFramework.Store.Duende/"]
COPY ["src/IdentityServer/Duende/Aguacongas.IdentityServer.MongoDb.Store.Duende/Aguacongas.IdentityServer.MongoDb.Store.Duende.csproj", "src/IdentityServer/Duende/Aguacongas.IdentityServer.MongoDb.Store.Duende/"]
COPY ["src/IdentityServer/Duende/Aguacongas.IdentityServer.RavenDb.Store.Duende/Aguacongas.IdentityServer.RavenDb.Store.Duende.csproj", "src/IdentityServer/Duende/Aguacongas.IdentityServer.RavenDb.Store.Duende/"]
COPY ["src/IdentityServer/Duende/Aguacongas.IdentityServer.Saml2p.Duende/Aguacongas.IdentityServer.Saml2p.Duende.csproj", "src/IdentityServer/Duende/Aguacongas.IdentityServer.Saml2p.Duende/"]
COPY ["src/IdentityServer/Duende/Aguacongas.IdentityServer.WsFederation.Duende/Aguacongas.IdentityServer.WsFederation.Duende.csproj", "src/IdentityServer/Duende/Aguacongas.IdentityServer.WsFederation.Duende/"]
RUN apt-get update -y
RUN apt-get install -y python3
RUN apt-get install -y gcc
RUN dotnet workload install wasm-tools
RUN dotnet restore "./src/Aguacongas.TheIdServer.Duende/./Aguacongas.TheIdServer.Duende.csproj"
COPY . .
WORKDIR "/src/src/Aguacongas.TheIdServer.Duende"
RUN dotnet build "./Aguacongas.TheIdServer.Duende.csproj" -c $BUILD_CONFIGURATION -o /app/build -p:FileVersion=$FILE_VERSION -p:SourceRevisionId=$SOURCE_VERSION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Aguacongas.TheIdServer.Duende.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -p:FileVersion=$FILE_VERSION -p:SourceRevisionId=$SOURCE_VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aguacongas.TheIdServer.Duende.dll"]