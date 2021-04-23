// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.RavenDb.Store;
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.Role;
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User;
using Aguacongas.IdentityServer.RavenDb.Store.Api;
using Aguacongas.IdentityServer.RavenDb.Store.ApiScope;
using Aguacongas.IdentityServer.RavenDb.Store.Client;
using Aguacongas.IdentityServer.RavenDb.Store.Identity;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        public static IServiceCollection AddIdentityServer4AdminRavenDbStores(this IServiceCollection services, Func<IServiceProvider, IDocumentStore> getDocumentStore = null, string dataBase = null)
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
                .AddTransient<IAdminStore<Entity.ApiApiScope>, ApiApiScopeStore>()
                .AddTransient<IAdminStore<Entity.ApiClaim>, ApiClaimStore>()
                .AddTransient<IAdminStore<Entity.ApiLocalizedResource>, ApiLocalizedResourceStore>()
                .AddTransient<IAdminStore<Entity.ApiProperty>, ApiPropertyStore>()
                .AddTransient<IAdminStore<Entity.ApiScope>, AdminStore<Entity.ApiScope>>()
                .AddTransient<IAdminStore<Entity.ApiScopeClaim>, ApiScopeClaimStore>()
                .AddTransient<IAdminStore<Entity.ApiScopeLocalizedResource>, ApiScopeLocalizedResourceStore>()
                .AddTransient<IAdminStore<Entity.ApiScopeProperty>, ApiScopePropertyStore>()
                .AddTransient<IAdminStore<Entity.ApiSecret>, ApiSecretStore>()
                .AddTransient<IAdminStore<Entity.AuthorizationCode>, AdminStore<Entity.AuthorizationCode>>()
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
                .AddTransient<IAdminStore<Entity.DeviceCode>, AdminStore<Entity.DeviceCode>>()
                .AddTransient<IAdminStore<Entity.ExternalProvider>>(p => new NotifyChangedExternalProviderStore(new AdminStore<Entity.ExternalProvider>(
                    p.GetRequiredService<ScopedAsynDocumentcSession>(),
                    p.GetRequiredService<ILogger<AdminStore<Entity.ExternalProvider>>>()),
                    p.GetRequiredService<IProviderClient>(),
                    p.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>(),
                    p.GetRequiredService<IAuthenticationSchemeOptionsSerializer>()))
                .AddTransient<IAdminStore<Entity.ExternalClaimTransformation>, ExternalClaimTransformationStore>()
                .AddTransient<IAdminStore<Entity.IdentityClaim>, IdentityClaimStore>()
                .AddTransient<IAdminStore<Entity.IdentityLocalizedResource>, IdentityLocalizedResourceStore>()
                .AddTransient<IAdminStore<Entity.IdentityProperty>, IdentityPropertyStore>()
                .AddTransient<IAdminStore<Entity.IdentityResource>, AdminStore<Entity.IdentityResource>>()
                .AddTransient<IAdminStore<Entity.LocalizedResource>, LocalizedResourceStore>()
                .AddTransient<IAdminStore<Entity.ProtectResource>, AdminStore<Entity.ProtectResource>>()
                .AddTransient<IAdminStore<Entity.Key>, AdminStore<Entity.Key>>()
                .AddTransient<IAdminStore<Entity.ReferenceToken>, AdminStore<Entity.ReferenceToken>>()
                .AddTransient<IAdminStore<Entity.RefreshToken>, AdminStore<Entity.RefreshToken>>()
                .AddTransient<IAdminStore<Entity.OneTimeToken>, AdminStore<Entity.OneTimeToken>>()
                .AddTransient<IAdminStore<Entity.UserConsent>, AdminStore<Entity.UserConsent>>()
                .AddTransient<IAdminStore<Entity.User>>(p => new CheckIdentityRulesUserStore(new AdminStore<Entity.User>(
                    p.GetRequiredService<ScopedAsynDocumentcSession>(),
                    p.GetRequiredService<ILogger<AdminStore<Entity.User>>>()),
                    p.GetRequiredService<UserManager<ApplicationUser>>()))
                .AddTransient<IAdminStore<Entity.UserLogin>, UserLoginStore>()
                .AddTransient<IAdminStore<Entity.UserClaim>, UserClaimStore>()
                .AddTransient<IAdminStore<Entity.UserRole>, UserRoleStore>()
                .AddTransient<IAdminStore<Entity.UserToken>, UserTokenStore>()
                .AddTransient<IAdminStore<Entity.Role>>(p => new CheckIdentityRulesRoleStore(new AdminStore<Entity.Role>(
                    p.GetRequiredService<ScopedAsynDocumentcSession>(),
                    p.GetRequiredService<ILogger<AdminStore<Entity.Role>>>()),
                    p.GetRequiredService<RoleManager<IdentityRole>>()))
                .AddTransient<IAdminStore<Entity.RoleClaim>, RoleClaimStore>();
                
        }
    }
}
