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
using Aguacongas.TheIdServer.Identity;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
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
                .AddTransient<IAdminStore<Entity.User>>(p => {
                    var userStore = new AdminStore<Entity.User>(
                        p.GetRequiredService<ScopedAsynDocumentcSession>(),
                        p.GetRequiredService<ILogger<AdminStore<Entity.User>>>());
                    var roleStore = new AdminStore<Entity.Role>(
                        p.GetRequiredService<ScopedAsynDocumentcSession>(),
                        p.GetRequiredService<ILogger<AdminStore<Entity.Role>>>());

                    return new CheckIdentityRulesUserStore(userStore,
                        new UserManager<ApplicationUser>(
                            new UserStore<ApplicationUser>(
                                roleStore,
                                p.GetRequiredService<IAdminStore<Entity.UserRole>>(),
                                new UserOnlyStore<ApplicationUser>(
                                    userStore,
                                    p.GetRequiredService<IAdminStore<Entity.UserClaim>>(),
                                    p.GetRequiredService<IAdminStore<Entity.UserLogin>>(),
                                    p.GetRequiredService<IAdminStore<Entity.UserToken>>(),
                                    p.GetService<IdentityErrorDescriber>()),
                                p.GetService<IdentityErrorDescriber>()),
                            p.GetRequiredService<IOptions<IdentityOptions>>(),
                            p.GetRequiredService<IPasswordHasher<ApplicationUser>>(),
                            p.GetRequiredService<IEnumerable<IUserValidator<ApplicationUser>>>(),
                            p.GetRequiredService<IEnumerable<IPasswordValidator<ApplicationUser>>>(),
                            p.GetRequiredService<ILookupNormalizer>(),
                            p.GetRequiredService<IdentityErrorDescriber>(),
                            p,
                            p.GetRequiredService<ILogger<UserManager<ApplicationUser>>>()));
                })
                .AddTransient<IAdminStore<Entity.UserLogin>, UserLoginStore>()
                .AddTransient<IAdminStore<Entity.UserClaim>, UserClaimStore>()
                .AddTransient<IAdminStore<Entity.UserRole>, UserRoleStore>()
                .AddTransient<IAdminStore<Entity.UserToken>, UserTokenStore>()
                .AddTransient<IAdminStore<Entity.Role>>(p => {
                    var store = new AdminStore<Entity.Role>(
                        p.GetRequiredService<ScopedAsynDocumentcSession>(),
                        p.GetRequiredService<ILogger<AdminStore<Entity.Role>>>());

                    return new CheckIdentityRulesRoleStore(store,
                        new RoleManager<IdentityRole>(
                            new RoleStore<IdentityRole>(
                                store,
                                p.GetRequiredService<IAdminStore<Entity.RoleClaim>>(),
                                p.GetService<IdentityErrorDescriber>()),
                            p.GetRequiredService<IEnumerable<IRoleValidator<IdentityRole>>>(),
                            p.GetRequiredService<ILookupNormalizer>(),
                            p.GetRequiredService<IdentityErrorDescriber>(),
                            p.GetRequiredService<ILogger<RoleManager<IdentityRole>>>()));
                    })
                .AddTransient<IAdminStore<Entity.RoleClaim>, RoleClaimStore>();
                
        }
    }
}
