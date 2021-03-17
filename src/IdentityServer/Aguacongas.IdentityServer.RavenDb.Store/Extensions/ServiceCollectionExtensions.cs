// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store;
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Linq;
using System.Reflection;
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
            var assembly = typeof(Entity.IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                t.GetInterface(nameof(Entity.IEntityId)) != null &&
                t.GetInterface(nameof(Entity.IGrant)) == null &&
                t.Name != nameof(Entity.AuthorizationCode) &&
                t.Name != nameof(Entity.DeviceCode) &&
                t.GetInterface(nameof(Entity.IRoleSubEntity)) == null &&
                t.GetInterface(nameof(Entity.IUserSubEntity)) == null);

            foreach (var entityType in entityTypeList)
            {
                var adminStoreType = typeof(AdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                var iAdminStoreType = typeof(IAdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(iAdminStoreType, adminStoreType);
            }

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
                .AddTransient<IUserStore<TUser>, UserStore<TUser, TRole>>()
                .AddTransient(p => new RavenDb.UserOnlyStore<TUser, string, UserClaim, IdentityUserLogin<string>, IdentityUserToken<string>>(p.GetRequiredService<ScopedAsynDocumentcSession>().Session, 
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
