# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Aguacongas.TheIdServer.BlazorApp.BFF/Aguacongas.TheIdServer.BlazorApp.BFF.csproj", "src/Aguacongas.TheIdServer.BlazorApp.BFF/"]
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
RUN dotnet restore "./src/Aguacongas.TheIdServer.BlazorApp.BFF/Aguacongas.TheIdServer.BlazorApp.BFF.csproj"
COPY . .
WORKDIR "/src/src/Aguacongas.TheIdServer.BlazorApp.BFF"
RUN dotnet build "./Aguacongas.TheIdServer.BlazorApp.BFF.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Aguacongas.TheIdServer.BlazorApp.BFF.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aguacongas.TheIdServer.BlazorApp.BFF.dll"]