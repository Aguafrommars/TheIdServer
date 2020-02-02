using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Services;
using IdentityServer4.Stores;
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
        /// Adds the identity server4 entity framework stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4EntityFrameworkStores<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
            where TContext : IdentityDbContext<IdentityUser>
        {
            return AddIdentityServer4EntityFrameworkStores<IdentityUser, IdentityRole, TContext>(services, optionsAction);
        }

        /// <summary>
        /// Adds the identity server4 entity framework stores.
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4EntityFrameworkStores<TUser, TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
            where TUser : IdentityUser, new()
            where TContext : IdentityDbContext<TUser>
        {
            return AddIdentityServer4EntityFrameworkStores<TUser, IdentityRole, TContext>(services, optionsAction);
        }
        /// <summary>
        /// Adds the identity server4 entity framework stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4EntityFrameworkStores<TUser, TRole, TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
            where TUser: IdentityUser, new()
            where TRole: IdentityRole, new()
            where TContext: IdentityDbContext<TUser>
        {
            var assembly = typeof(IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                t.GetInterface("IEntityId") != null &&
                t.GetInterface("IRoleSubEntity") == null &&
                t.GetInterface("IUserSubEntity") == null);

            foreach (var entityType in entityTypeList)
            {
                var adminStoreType = typeof(AdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                var iAdminStoreType = typeof(IAdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(iAdminStoreType, adminStoreType);
            }

            return services.AddDbContext<IdentityServerDbContext>(optionsAction)
                .AddScoped<IdentityDbContext<TUser>>(p => p.GetRequiredService<TContext>())
                .AddScoped(p => p.GetRequiredService<TContext>() as IdentityDbContext<TUser, TRole, string>)
                .AddTransient<IClientStore, ClientStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<IAdminStore<User>, IdentityUserStore<TUser>>()
                .AddTransient<IAdminStore<UserLogin>, IdentityUserLoginStore<TUser>>()
                .AddTransient<IAdminStore<UserClaim>, IdentityUserClaimStore<TUser>>()
                .AddTransient<IAdminStore<UserRole>, IdentityUserRoleStore<TUser>>()
                .AddTransient<IAdminStore<UserToken>, IdentityUserTokenStore<TUser>>()
                .AddTransient<IAdminStore<Role>, IdentityRoleStore<TUser, TRole>>()
                .AddTransient<IAdminStore<RoleClaim>, IdentityRoleClaimStore<TUser, TRole>>();
        }

        public static IServiceCollection AddOperationalEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            return services.AddDbContext<OperationalDbContext>(optionsAction)
                .AddTransient<AuthorizationCodeStore>()
                .AddTransient<RefreshTokenStore>()
                .AddTransient<ReferenceTokenStore>()
                .AddTransient<UserConsentStore>()
                .AddTransient<DeviceFlowStore>()
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
