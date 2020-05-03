using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

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
        public static IServiceCollection AddIdentityServer4AdminEntityFrameworkStores<TContext>(this IServiceCollection services)
            where TContext : IdentityDbContext<IdentityUser>
        {
            return AddIdentityServer4AdminEntityFrameworkStores<IdentityUser, IdentityRole, TContext>(services);
        }

        /// <summary>
        /// Adds the identity server4 admin entity framework stores.
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4AdminEntityFrameworkStores<TUser, TContext>(this IServiceCollection services)
            where TUser : IdentityUser, new()
            where TContext : IdentityDbContext<TUser>
        {
            return AddIdentityServer4AdminEntityFrameworkStores<TUser, IdentityRole, TContext>(services);
        }


        /// <summary>
        /// Adds the identity server4 admin entity framework stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4AdminEntityFrameworkStores<TUser, TRole, TContext>(this IServiceCollection services)
            where TUser: IdentityUser, new()
            where TRole: IdentityRole, new()
            where TContext: IdentityDbContext<TUser>
        {
            var assembly = typeof(IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                t.GetInterface("IEntityId") != null &&
                t.GetInterface("IGrant") == null &&
                t.Name != nameof(AuthorizationCode) &&
                t.Name != nameof(DeviceCode) &&
                t.GetInterface("IRoleSubEntity") == null &&
                t.GetInterface("IUserSubEntity") == null);

            foreach (var entityType in entityTypeList)
            {
                var adminStoreType = typeof(AdminStore<,>)
                        .MakeGenericType(entityType.GetTypeInfo(), typeof(ConfigurationDbContext).GetTypeInfo()).GetTypeInfo();
                var iAdminStoreType = typeof(IAdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(iAdminStoreType, adminStoreType);
            }

            return services.AddScoped<IdentityDbContext<TUser>>(p => p.GetRequiredService<TContext>())
                .AddScoped(p => p.GetRequiredService<TContext>() as IdentityDbContext<TUser, TRole, string>)
                .AddTransient<IAdminStore<User>, IdentityUserStore<TUser>>()
                .AddTransient<IAdminStore<UserLogin>, IdentityUserLoginStore<TUser>>()
                .AddTransient<IAdminStore<UserClaim>, IdentityUserClaimStore<TUser>>()
                .AddTransient<IAdminStore<UserRole>, IdentityUserRoleStore<TUser>>()
                .AddTransient<IAdminStore<UserToken>, IdentityUserTokenStore<TUser>>()
                .AddTransient<IAdminStore<Role>, IdentityRoleStore<TUser, TRole>>()
                .AddTransient<IAdminStore<RoleClaim>, IdentityRoleClaimStore<TUser, TRole>>()
                .AddTransient<IAdminStore<ExternalProvider>, ExternalProviderStore>()
                .AddTransient<IExternalProviderKindStore, ExternalProviderKindStore>();
        }

        public static IServiceCollection AddConfigurationEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            return services.AddDbContext<ConfigurationDbContext>(optionsAction)
                .AddTransient<IClientStore, ClientStore>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>();
        }

        public static IServiceCollection AddOperationalEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            return services.AddDbContext<OperationalDbContext>(optionsAction)
                .AddTransient<AuthorizationCodeStore>()
                .AddTransient<RefreshTokenStore>()
                .AddTransient<ReferenceTokenStore>()
                .AddTransient<UserConsentStore>()
                .AddTransient<DeviceFlowStore>()
                .AddTransient<IPersistentGrantSerializer, PersistentGrantSerializer>()
                .AddTransient<IAuthorizationCodeStore>(p => p.GetRequiredService<AuthorizationCodeStore>())
                .AddTransient<IAdminStore<AuthorizationCode>>(p => p.GetRequiredService<AuthorizationCodeStore>())
                .AddTransient<IRefreshTokenStore>(p => p.GetRequiredService<RefreshTokenStore>())
                .AddTransient<IAdminStore<RefreshToken>>(p => p.GetRequiredService<RefreshTokenStore>())
                .AddTransient<IReferenceTokenStore>(p => p.GetRequiredService<ReferenceTokenStore>())
                .AddTransient<IAdminStore<ReferenceToken>>(p => p.GetRequiredService<ReferenceTokenStore>())
                .AddTransient<IUserConsentStore>(p => p.GetRequiredService<UserConsentStore>())
                .AddTransient<IAdminStore<UserConsent>>(p => p.GetRequiredService<UserConsentStore>())
                .AddTransient<IGetAllUserConsentStore, GetAllUserConsentStore>()
                .AddTransient<IDeviceFlowStore>(p => p.GetRequiredService<DeviceFlowStore>())
                .AddTransient<IAdminStore<DeviceCode>> (p => p.GetRequiredService<DeviceFlowStore>());
        }
    }
}
