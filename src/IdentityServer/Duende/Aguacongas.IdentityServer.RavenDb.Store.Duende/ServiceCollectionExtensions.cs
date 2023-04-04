// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store;
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.RelyingParty;
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.Role;
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User;
using Aguacongas.IdentityServer.RavenDb.Store.Api;
using Aguacongas.IdentityServer.RavenDb.Store.ApiScope;
using Aguacongas.IdentityServer.RavenDb.Store.Client;
using Aguacongas.IdentityServer.RavenDb.Store.Identity;
using Aguacongas.IdentityServer.Store;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the identity server4 admin entity framework stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddTheIdServerRavenDbStores(this IServiceCollection services, Func<IServiceProvider, IDocumentStore> getDocumentStore = null, string dataBase = null)
        {
            if (getDocumentStore == null)
            {
                getDocumentStore = p => p.GetRequiredService<IDocumentStore>();
            }

            return services.AddScoped(p =>
                {
                    var session = getDocumentStore(p).OpenAsyncSession(new SessionOptions
                    {
                        Database = dataBase
                    });
                    var adv = session.Advanced;
                    adv.UseOptimisticConcurrency = false;
                    adv.MaxNumberOfRequestsPerSession = int.MaxValue;
                    return new ScopedAsynDocumentcSession(session);
                })
                .AddTransient<ApiApiScopeStore>()
                .AddTransient<ApiClaimStore>()
                .AddTransient<ApiLocalizedResourceStore>()
                .AddTransient<ApiPropertyStore>()
                .AddTransient<AdminStore<Entity.ApiScope>>()
                .AddTransient<ApiScopeClaimStore>()
                .AddTransient<ApiScopeLocalizedResourceStore>()
                .AddTransient<ApiScopePropertyStore>()
                .AddTransient<ApiSecretStore>()
                .AddTransient<AdminStore<Entity.AuthorizationCode>>()
                .AddTransient<AdminStore<Entity.Client>>()
                .AddTransient<ClientAllowedIdentityTokenSigningAlgorithmStore>()
                .AddTransient<ClientClaimStore>()
                .AddTransient<ClientGrantTypeStore>()
                .AddTransient<ClientIdpRestrictionStore>()
                .AddTransient<ClientLocalizedResourceStore>()
                .AddTransient<ClientPropertyStore>()
                .AddTransient<ClientScopeStore>()
                .AddTransient<ClientSecretStore>()
                .AddTransient<ClientUriStore>()
                .AddTransient<AdminStore<Entity.Culture>>()
                .AddTransient<AdminStore<Entity.DeviceCode>>()
                .AddTransient<AdminStore<Entity.ExternalProvider>>()
                .AddTransient<ExternalClaimTransformationStore>()
                .AddTransient<IdentityClaimStore>()
                .AddTransient<IdentityLocalizedResourceStore>()
                .AddTransient<IdentityPropertyStore>()
                .AddTransient<AdminStore<Entity.IdentityResource>>()
                .AddTransient<LocalizedResourceStore>()
                .AddTransient<AdminStore<Entity.BackChannelAuthenticationRequest>>()
                .AddTransient<AdminStore<Entity.ProtectResource>>()
                .AddTransient<AdminStore<Entity.Key>>()
                .AddTransient<AdminStore<Entity.ReferenceToken>>()
                .AddTransient<AdminStore<Entity.RefreshToken>>()
                .AddTransient<AdminStore<Entity.OneTimeToken>>()
                .AddTransient<AdminStore<Entity.UserConsent>>()
                .AddTransient<AdminStore<Entity.User>>()
                .AddTransient<AdminStore<Entity.Role>>()
                .AddTransient<RoleClaimStore>()
                .AddTransient<UserClaimStore>()
                .AddTransient<UserLoginStore>()
                .AddTransient<UserTokenStore>()
                .AddTransient<UserRoleStore>()
                .AddTransient<UserSessionStore>()
                .AddTransient<AdminStore<Entity.RelyingParty>>()
                .AddTransient<RelyingPartyClaimMappingStore>()
                .AddTransient<AdminStore<Entity.Saml2pArtifact>>()
                .AddTransient<IAdminStore<Entity.ApiApiScope>, CacheAdminStore<ApiApiScopeStore, Entity.ApiApiScope>>()
                .AddTransient<IAdminStore<Entity.ApiClaim>, CacheAdminStore<ApiClaimStore, Entity.ApiClaim>>()
                .AddTransient<IAdminStore<Entity.ApiLocalizedResource>, CacheAdminStore<ApiLocalizedResourceStore, Entity.ApiLocalizedResource>>()
                .AddTransient<IAdminStore<Entity.ApiProperty>, CacheAdminStore<ApiPropertyStore, Entity.ApiProperty>>()
                .AddTransient<IAdminStore<Entity.ApiScope>, CacheAdminStore<AdminStore<Entity.ApiScope>, Entity.ApiScope>>()
                .AddTransient<IAdminStore<Entity.ApiScopeClaim>, CacheAdminStore<ApiScopeClaimStore, Entity.ApiScopeClaim>>()
                .AddTransient<IAdminStore<Entity.ApiScopeLocalizedResource>, CacheAdminStore<ApiScopeLocalizedResourceStore, Entity.ApiScopeLocalizedResource>>()
                .AddTransient<IAdminStore<Entity.ApiScopeProperty>, CacheAdminStore<ApiScopePropertyStore, Entity.ApiScopeProperty>>()
                .AddTransient<IAdminStore<Entity.ApiSecret>, CacheAdminStore<ApiSecretStore, Entity.ApiSecret>>()
                .AddTransient<IAdminStore<Entity.AuthorizationCode>, CacheAdminStore<AdminStore<Entity.AuthorizationCode>, Entity.AuthorizationCode>>()
                .AddTransient<IAdminStore<Entity.Client>, CacheAdminStore<AdminStore<Entity.Client>, Entity.Client>>()
                .AddTransient<IAdminStore<Entity.ClientAllowedIdentityTokenSigningAlgorithm>, CacheAdminStore<ClientAllowedIdentityTokenSigningAlgorithmStore, Entity.ClientAllowedIdentityTokenSigningAlgorithm>>()
                .AddTransient<IAdminStore<Entity.ClientClaim>, CacheAdminStore<ClientClaimStore, Entity.ClientClaim>>()
                .AddTransient<IAdminStore<Entity.ClientGrantType>, CacheAdminStore<ClientGrantTypeStore, Entity.ClientGrantType>>()
                .AddTransient<IAdminStore<Entity.ClientIdpRestriction>, CacheAdminStore<ClientIdpRestrictionStore, Entity.ClientIdpRestriction>>()
                .AddTransient<IAdminStore<Entity.ClientLocalizedResource>, CacheAdminStore<ClientLocalizedResourceStore, Entity.ClientLocalizedResource>>()
                .AddTransient<IAdminStore<Entity.ClientProperty>, CacheAdminStore<ClientPropertyStore, Entity.ClientProperty>>()
                .AddTransient<IAdminStore<Entity.ClientScope>, CacheAdminStore<ClientScopeStore, Entity.ClientScope>>()
                .AddTransient<IAdminStore<Entity.ClientSecret>, CacheAdminStore<ClientSecretStore, Entity.ClientSecret>>()
                .AddTransient<IAdminStore<Entity.ClientUri>, CacheAdminStore<ClientUriStore, Entity.ClientUri>>()
                .AddTransient<IAdminStore<Entity.Culture>, CacheAdminStore<AdminStore<Entity.Culture>, Entity.Culture>>()
                .AddTransient<IAdminStore<Entity.DeviceCode>, CacheAdminStore<AdminStore<Entity.DeviceCode>, Entity.DeviceCode>>()
                .AddTransient<IAdminStore<Entity.ExternalClaimTransformation>, CacheAdminStore<ExternalClaimTransformationStore, Entity.ExternalClaimTransformation>>()
                .AddTransient<IAdminStore<Entity.IdentityClaim>, CacheAdminStore<IdentityClaimStore, Entity.IdentityClaim>>()
                .AddTransient<IAdminStore<Entity.IdentityLocalizedResource>, CacheAdminStore<IdentityLocalizedResourceStore, Entity.IdentityLocalizedResource>>()
                .AddTransient<IAdminStore<Entity.IdentityProperty>, CacheAdminStore<IdentityPropertyStore, Entity.IdentityProperty>>()
                .AddTransient<IAdminStore<Entity.IdentityResource>, CacheAdminStore<AdminStore<Entity.IdentityResource>, Entity.IdentityResource>>()
                .AddTransient<IAdminStore<Entity.LocalizedResource>, CacheAdminStore<LocalizedResourceStore, Entity.LocalizedResource>>()
                .AddTransient<IAdminStore<Entity.ProtectResource>, CacheAdminStore<AdminStore<Entity.ProtectResource>, Entity.ProtectResource>>()
                .AddTransient<IAdminStore<Entity.Key>, CacheAdminStore<AdminStore<Entity.Key>, Entity.Key>>()
                .AddTransient<IAdminStore<Entity.BackChannelAuthenticationRequest>, CacheAdminStore<AdminStore<Entity.BackChannelAuthenticationRequest>, Entity.BackChannelAuthenticationRequest>>()
                .AddTransient<IAdminStore<Entity.ReferenceToken>, CacheAdminStore<AdminStore<Entity.ReferenceToken>, Entity.ReferenceToken>>()
                .AddTransient<IAdminStore<Entity.RefreshToken>, CacheAdminStore<AdminStore<Entity.RefreshToken>, Entity.RefreshToken>>()
                .AddTransient<IAdminStore<Entity.OneTimeToken>, CacheAdminStore<AdminStore<Entity.OneTimeToken>, Entity.OneTimeToken>>()
                .AddTransient<IAdminStore<Entity.UserConsent>, CacheAdminStore<AdminStore<Entity.UserConsent>, Entity.UserConsent>>()
                .AddTransient<IAdminStore<Entity.User>, CacheAdminStore<AdminStore<Entity.User>, Entity.User>>()
                .AddTransient<IAdminStore<Entity.UserLogin>, CacheAdminStore<UserLoginStore, Entity.UserLogin>>()
                .AddTransient<IAdminStore<Entity.UserClaim>, CacheAdminStore<UserClaimStore, Entity.UserClaim>>()
                .AddTransient<IAdminStore<Entity.UserRole>, CacheAdminStore<UserRoleStore, Entity.UserRole>>()
                .AddTransient<IAdminStore<Entity.UserToken>, CacheAdminStore<UserTokenStore, Entity.UserToken>>()
                .AddTransient<IAdminStore<Entity.UserSession>, CacheAdminStore<UserSessionStore, Entity.UserSession>>()
                .AddTransient<IAdminStore<Entity.Role>, CacheAdminStore<AdminStore<Entity.Role>, Entity.Role>>()
                .AddTransient<IAdminStore<Entity.RoleClaim>, CacheAdminStore<RoleClaimStore, Entity.RoleClaim>>()
                .AddTransient<IAdminStore<Entity.RelyingParty>, CacheAdminStore<AdminStore<Entity.RelyingParty>, Entity.RelyingParty>>()
                .AddTransient<IAdminStore<Entity.RelyingPartyClaimMapping>, CacheAdminStore<RelyingPartyClaimMappingStore, Entity.RelyingPartyClaimMapping>>()
                .AddTransient<IAdminStore<Entity.Saml2pArtifact>, CacheAdminStore<AdminStore<Entity.Saml2pArtifact>, Entity.Saml2pArtifact>>()
                .AddTransient<CacheAdminStore<AdminStore<Entity.User>, Entity.User>>()
                .AddTransient<CacheAdminStore<AdminStore<Entity.Role>, Entity.Role>>()
                .AddTransient<CacheAdminStore<AdminStore<Entity.ExternalProvider>, Entity.ExternalProvider>>()
                .AddTransient<CacheAdminStore<AdminStore<Entity.Saml2pArtifact>, Entity.Saml2pArtifact>>();                
        }
    }
}
