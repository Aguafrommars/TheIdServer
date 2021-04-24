// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Identity;
using Aguacongas.TheIdServer.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the identity provider store in DI container.
        /// </summary>
        /// <param name="services">The collection of service.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityProviderStore(this IServiceCollection services)
        {
            return services
                .AddSingleton(p => new OAuthTokenManager(p.GetRequiredService<HttpClient>(), p.GetRequiredService<IOptions<IdentityServerOptions>>()))
                .AddTransient<OAuthDelegatingHandler>()
                .AddTransient(p => new HttpClient(p.GetRequiredService<HttpClientHandler>()));
        }

        public static IServiceCollection AddConfigurationStores(this IServiceCollection services)
        {
            services.TryAddScoped(typeof(IFlushableCache<>), typeof(FlushableCache<>));

            return services.AddTransient<ClientStore>()
                .AddTransient<IClientStore, ValidatingClientStore<ClientStore>>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>()
                .AddTransient<IExternalProviderKindStore, ExternalProviderKindStore<SchemeDefinition>>();
        }

        public static IServiceCollection AddOperationalStores(this IServiceCollection services)
        {
            return services.AddTransient<AuthorizationCodeStore>()
                .AddTransient<RefreshTokenStore>()
                .AddTransient<ReferenceTokenStore>()
                .AddTransient<UserConsentStore>()
                .AddTransient<DeviceFlowStore>()
                .AddTransient<IPersistentGrantSerializer, PersistentGrantSerializer>()
                .AddTransient<IAuthorizationCodeStore>(p => p.GetRequiredService<AuthorizationCodeStore>())
                .AddTransient<IRefreshTokenStore>(p => p.GetRequiredService<RefreshTokenStore>())
                .AddTransient<IReferenceTokenStore>(p => p.GetRequiredService<ReferenceTokenStore>())
                .AddTransient<IUserConsentStore>(p => p.GetRequiredService<UserConsentStore>())
                .AddTransient<IDeviceFlowStore>(p => p.GetRequiredService<DeviceFlowStore>());
        }

        public static IServiceCollection AddRulesCheckStores<TUserStore, TRoleStore>(this IServiceCollection services)
            where TUserStore: IAdminStore<User>
            where TRoleStore: IAdminStore<Role>
        {
            return services.AddTransient<IAdminStore<User>>(p =>
                {
                    var userStore = p.GetRequiredService<TUserStore>();
                    var roleStore = p.GetRequiredService<TRoleStore>();

                    return new CheckIdentityRulesUserStore<TUserStore>(userStore,
                        new UserManager<ApplicationUser>(
                            new UserStore<ApplicationUser>(
                                roleStore,
                                p.GetRequiredService<IAdminStore<UserRole>>(),
                                new UserOnlyStore<ApplicationUser>(
                                    userStore,
                                    p.GetRequiredService<IAdminStore<UserClaim>>(),
                                    p.GetRequiredService<IAdminStore<UserLogin>>(),
                                    p.GetRequiredService<IAdminStore<UserToken>>(),
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
                  .AddTransient<IAdminStore<Role>>(p =>
                  {
                      var store = p.GetRequiredService<TRoleStore>();

                      return new CheckIdentityRulesRoleStore<TRoleStore>(store,
                          new RoleManager<IdentityRole>(
                              new RoleStore<IdentityRole>(
                                  store,
                                  p.GetRequiredService<IAdminStore<RoleClaim>>(),
                                  p.GetService<IdentityErrorDescriber>()),
                              p.GetRequiredService<IEnumerable<IRoleValidator<IdentityRole>>>(),
                              p.GetRequiredService<ILookupNormalizer>(),
                              p.GetRequiredService<IdentityErrorDescriber>(),
                              p.GetRequiredService<ILogger<RoleManager<IdentityRole>>>()));
                  });
        }
    }
}
