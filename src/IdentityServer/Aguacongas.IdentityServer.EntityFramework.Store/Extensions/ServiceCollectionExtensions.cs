using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
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
        public static IServiceCollection AddIdentityServer4EntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            return AddIdentityServer4EntityFrameworkStores<IdentityUser, IdentityRole>(services, optionsAction);
        }

        /// <summary>
        /// Adds the identity server4 entity framework stores.
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4EntityFrameworkStores<TUser>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
            where TUser : IdentityUser
        {
            return AddIdentityServer4EntityFrameworkStores<TUser, IdentityRole>(services, optionsAction);
        }
        /// <summary>
        /// Adds the identity server4 entity framework stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4EntityFrameworkStores<TUser, TRole>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
            where TUser: IdentityUser
            where TRole: IdentityRole
        {
            var assembly = typeof(IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract &&
                t.GetInterface("IEntityId") != null);

            foreach (var entityType in entityTypeList)
            {
                var adminStoreType = typeof(AdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                var iAdminStoreType = typeof(IAdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(iAdminStoreType, adminStoreType);
            }

            return services.AddDbContext<IdentityServerDbContext>(optionsAction)
                .AddTransient<IClientStore, ClientStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<IAuthorizationCodeStore, AuthorizationCodeStore>()
                .AddTransient<IRefreshTokenStore, RefreshTokenStore>()
                .AddTransient<IReferenceTokenStore, ReferenceTokenStore>()
                .AddTransient<IUserConsentStore, UserConsentStore>()
                .AddTransient<IGetAllUserConsentStore, GetAllUserConsentStore>()
                .AddTransient<IIdentityUserStore<TUser>, IdentityUserStore<TUser>>()
                .AddTransient<IIdentityRoleStore<TRole>, IdentityRoleStore<TRole>>();
        }
    }
}
