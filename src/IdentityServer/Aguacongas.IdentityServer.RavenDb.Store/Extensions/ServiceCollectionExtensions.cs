// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store;
using Aguacongas.IdentityServer.RavenDb.Store.Api;
using Aguacongas.IdentityServer.RavenDb.Store.ApiScope;
using Aguacongas.IdentityServer.RavenDb.Store.Client;
using Aguacongas.IdentityServer.RavenDb.Store.Identity;
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using Entity = Aguacongas.IdentityServer.Store.Entity;
using RavenDb = Aguacongas.Identity.RavenDb;

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
        public static IServiceCollection AddIdentityServer4AdminRavenDbkStores(this IServiceCollection services, Func<IServiceProvider, IDocumentStore> getDocumentStore = null, string dataBase = null)
        {
            return AddIdentityServer4AdminRavenDbkStores<IdentityUser, IdentityRole>(services, getDocumentStore, dataBase);
        }

        /// <summary>
        /// Adds the identity server4 admin entity framework stores.
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4AdminRavenDbkStores<TUser>(this IServiceCollection services, Func<IServiceProvider, IDocumentStore> getDocumentStore = null, string dataBase = null)
            where TUser : IdentityUser, new()
        {
            return AddIdentityServer4AdminRavenDbkStores<TUser, IdentityRole>(services, getDocumentStore, dataBase);
        }


        /// <summary>
        /// Adds the identity server4 admin entity framework stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4AdminRavenDbkStores<TUser, TRole>(this IServiceCollection services, Func<IServiceProvider, IDocumentStore> getDocumentStore = null, string dataBase = null)
            where TUser: IdentityUser, new()
            where TRole: IdentityRole, new()
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
                    adv.UseOptimisticConcurrency = true;
                    adv.MaxNumberOfRequestsPerSession = int.MaxValue;
                    return new ScopedAsynDocumentcSession(session);
                })
                .AddTransient<IAdminStore<Entity.ApiApiScope>, ApiApiScopeStore>()
                .AddTransient<IAdminStore<Entity.ApiClaim>, ApiClaimStore>()
                .AddTransient<IAdminStore<Entity.ApiLocalizedResource>, ApiLocalizedResourceStore>()
                .AddTransient<IAdminStore<Entity.ApiProperty>, ApiPropertyStore>()
                .AddTransient<IAdminStore<Entity.ApiScope>, AdminStore<Entity.ApiScope>>()
                .AddTransient<IAdminStore<Entity.ApiScopeClaim>, ApiScopeClaimStore>()
                .AddTransient<IAdminStore<Entity.ApiScopeLocalizedResource>, ApiScopeLocalizedResourceStore>()
                .AddTransient<IAdminStore<Entity.ApiScopeProperty>, ApiScopePropertyStore>()
                .AddTransient<IAdminStore<Entity.ApiSecret>, ApiSecretStore>()
                .AddTransient<IAdminStore<Entity.AuthorizationCode>, AuthorizationCodeStore>()
                .AddTransient<IAdminStore<Entity.Client>, AdminStore<Entity.Client>>()
                .AddTransient<IAdminStore<Entity.ClientClaim>, ClientClaimStore>()
                .AddTransient<IAdminStore<Entity.ClientGrantType>, ClientGrantTypeStore>()
                .AddTransient<IAdminStore<Entity.ClientIdpRestriction>, ClientIdpRestrictionStore>()
                .AddTransient<IAdminStore<Entity.ClientLocalizedResource>, ClientLocalizedResourceStore>()
                .AddTransient<IAdminStore<Entity.ClientProperty>, ClientPropertyStore>()
                .AddTransient<IAdminStore<Entity.ClientScope>, ClientScopeStore>()
                .AddTransient<IAdminStore<Entity.ClientSecret>, ClientSecretStore>()
                .AddTransient<IAdminStore<Entity.ClientUri>, ClientUriStore>()
                .AddTransient<IAdminStore<Entity.Culture>, AdminStore<Entity.Culture>>()
                .AddTransient<IAdminStore<Entity.DeviceCode>, DeviceFlowStore>()
                .AddTransient<IAdminStore<Entity.ExternalClaimTransformation>, AdminStore<Entity.ExternalClaimTransformation>>()
                .AddTransient<IAdminStore<Entity.ExternalProvider>, ExternalProviderStore>()
                .AddTransient<IAdminStore<Entity.IdentityClaim>, IdentityClaimStore>()
                .AddTransient<IAdminStore<Entity.IdentityLocalizedResource>, IdentityLocalizedResourceStore>()
                .AddTransient<IAdminStore<Entity.IdentityProperty>, IdentityPropertyStore>()
                .AddTransient<IAdminStore<Entity.IdentityResource>, AdminStore<Entity.IdentityResource>>()
                .AddTransient<IAdminStore<Entity.LocalizedResource>, AdminStore<Entity.LocalizedResource>>()
                .AddTransient<IAdminStore<Entity.ProtectResource>, AdminStore<Entity.ProtectResource>>()
                .AddTransient<IAdminStore<Entity.Key>, AdminStore<Entity.Key>>()
                .AddTransient<IAdminStore<Entity.ReferenceToken>, ReferenceTokenStore>()
                .AddTransient<IAdminStore<Entity.RefreshToken>, RefreshTokenStore>()
                .AddTransient<IAdminStore<Entity.OneTimeToken>, AdminStore<Entity.OneTimeToken>>()
                .AddTransient<IAdminStore<Entity.UserConsent>, UserConsentStore>()
                .AddTransient<IUserStore<TUser>, UserStore<TUser, TRole>>()
                .AddTransient(p => new RavenDb.UserOnlyStore<TUser, string, Aguacongas.IdentityServer.RavenDb.Store.UserClaim, IdentityUserLogin<string>, IdentityUserToken<string>>(
                    p.GetRequiredService<ScopedAsynDocumentcSession>().Session, 
                    p.GetRequiredService<IdentityErrorDescriber>()))
                .AddTransient<IAdminStore<Entity.User>, IdentityUserStore<TUser>>()
                .AddTransient<IAdminStore<Entity.UserLogin>, IdentityUserLoginStore<TUser>>()
                .AddTransient<IAdminStore<Entity.UserClaim>, IdentityUserClaimStore<TUser>>()
                .AddTransient<IAdminStore<Entity.UserRole>, IdentityUserRoleStore<TUser>>()
                .AddTransient<IAdminStore<Entity.UserToken>, IdentityUserTokenStore<TUser>>()
                .AddTransient<IAdminStore<Entity.Role>, IdentityRoleStore<TUser, TRole>>()
                .AddTransient<IAdminStore<Entity.RoleClaim>, IdentityRoleClaimStore<TUser, TRole>>()
                .AddTransient<IAdminStore<Entity.ExternalProvider>, ExternalProviderStore>()
                .AddTransient<IExternalProviderKindStore, ExternalProviderKindStore>();
        }

        public static IServiceCollection AddConfigurationRavenDbkStores(this IServiceCollection services)
        {
            return services
                .AddTransient<IClientStore, ClientStore>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>();
        }

        public static IServiceCollection AddOperationalRavenDbStores(this IServiceCollection services)
        {
            return services
                .AddTransient<AuthorizationCodeStore>()
                .AddTransient<RefreshTokenStore>()
                .AddTransient<ReferenceTokenStore>()
                .AddTransient<UserConsentStore>()
                .AddTransient<DeviceFlowStore>()
                .AddTransient<IAdminStore<Entity.OneTimeToken>, AdminStore<Entity.OneTimeToken>>()
                .AddTransient<IPersistentGrantSerializer, PersistentGrantSerializer>()
                .AddTransient<IAuthorizationCodeStore>(p => p.GetRequiredService<AuthorizationCodeStore>())
                .AddTransient<IAdminStore<Entity.AuthorizationCode>>(p => p.GetRequiredService<AuthorizationCodeStore>())
                .AddTransient<IRefreshTokenStore>(p => p.GetRequiredService<RefreshTokenStore>())
                .AddTransient<IAdminStore<Entity.RefreshToken>>(p => p.GetRequiredService<RefreshTokenStore>())
                .AddTransient<IReferenceTokenStore>(p => p.GetRequiredService<ReferenceTokenStore>())
                .AddTransient<IAdminStore<Entity.ReferenceToken>>(p => p.GetRequiredService<ReferenceTokenStore>())
                .AddTransient<IUserConsentStore>(p => p.GetRequiredService<UserConsentStore>())
                .AddTransient<IAdminStore<Entity.UserConsent>>(p => p.GetRequiredService<UserConsentStore>())
                .AddTransient<IDeviceFlowStore>(p => p.GetRequiredService<DeviceFlowStore>())
                .AddTransient<IAdminStore<Entity.DeviceCode>> (p => p.GetRequiredService<DeviceFlowStore>());
        }
    }
}
