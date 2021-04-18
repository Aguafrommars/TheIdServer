// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using AutoMapper.Internal;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
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
        public static IServiceCollection AddIdentityServer4AdminEntityFrameworkStores<TContext>(this IServiceCollection services)
            where TContext : IdentityDbContext<IdentityUser, IdentityRole>
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
            where TContext : IdentityDbContext<TUser, IdentityRole>
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
            where TContext: IdentityDbContext<TUser, TRole>
        {
            return services.AddScoped(p => p.GetRequiredService<TContext>() as IdentityDbContext<TUser>)
                .AddScoped(p => p.GetRequiredService<TContext>() as IdentityDbContext<TUser, TRole>)    
                .AddTransient<IUserStore<TUser>, UserStore<TUser, TRole, TContext>>()
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

        public static IServiceCollection AddConfigurationEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            var dbContextType = typeof(ConfigurationDbContext);
            foreach(var property in dbContextType.GetProperties().Where(p => p.PropertyType.ImplementsGenericInterface(typeof(IQueryable<>))))
            {
                var entityType = property.PropertyType.GetGenericArguments()[0];
                var adminStoreType = typeof(AdminStore<,>)
                        .MakeGenericType(entityType.GetTypeInfo(), dbContextType.GetTypeInfo()).GetTypeInfo();
                var iAdminStoreType = typeof(IAdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(iAdminStoreType, adminStoreType);
            }
            return services.AddDbContext<ConfigurationDbContext>(optionsAction)
                .AddConfigurationStores<SchemeDefinition>();
        }

        public static IServiceCollection AddOperationalEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            var dbContextType = typeof(OperationalDbContext);
            foreach (var property in dbContextType.GetProperties().Where(p => p.PropertyType.ImplementsGenericInterface(typeof(IQueryable<>)) && p.PropertyType.GetGenericArguments()[0].IsAssignableTo(typeof(Entity.IEntityId))))
            {
                var entityType = property.PropertyType.GetGenericArguments()[0];
                var adminStoreType = typeof(AdminStore<,>)
                        .MakeGenericType(entityType.GetTypeInfo(), dbContextType.GetTypeInfo()).GetTypeInfo();
                var iAdminStoreType = typeof(IAdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(iAdminStoreType, adminStoreType);
            }
            return services.AddDbContext<OperationalDbContext>(optionsAction)
                .AddOperationalStores();
        }
    }
}
