// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Identity;
using Aguacongas.TheIdServer.Models;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
        public static IServiceCollection AddIdentityServer4AdminEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            AddStoresForContext(services, typeof(ApplicationDbContext));
            return services.AddDbContext<ApplicationDbContext>(optionsAction)
                .AddTransient<IAdminStore<User>>(p => {
                    var userStore = p.GetRequiredService<CacheAdminStore<AdminStore<User, ApplicationDbContext>, User>>();
                    var roleStore = p.GetRequiredService<CacheAdminStore<AdminStore<Role, ApplicationDbContext>, Role>>();

                    return new CheckIdentityRulesUserStore<CacheAdminStore<AdminStore<User, ApplicationDbContext>, User>>(userStore,
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
                .AddTransient<IAdminStore<Role>>(p => {
                    var store = p.GetRequiredService<CacheAdminStore<AdminStore<Role, ApplicationDbContext>, Role>>(); 
                    return new CheckIdentityRulesRoleStore<CacheAdminStore<AdminStore<Role, ApplicationDbContext>, Role>>(store,
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

        public static IServiceCollection AddConfigurationEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            AddStoresForContext(services, typeof(ConfigurationDbContext));            
            return services.AddDbContext<ConfigurationDbContext>(optionsAction)
                .AddConfigurationStores();
        }

        public static IServiceCollection AddOperationalEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            AddStoresForContext(services, typeof(OperationalDbContext));
            return services.AddDbContext<OperationalDbContext>(optionsAction)
                .AddOperationalStores();
        }

        private static void AddStoresForContext(IServiceCollection services, Type dbContextType)
        {
            foreach (var property in dbContextType.GetProperties().Where(p => p.PropertyType.ImplementsGenericInterface(typeof(IQueryable<>)) &&
                p.PropertyType.GetGenericArguments()[0].IsAssignableTo(typeof(IEntityId))))
            {
                var entityType = property.PropertyType.GetGenericArguments()[0];
                var adminStoreType = typeof(AdminStore<,>)
                        .MakeGenericType(entityType.GetTypeInfo(), dbContextType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(adminStoreType);

                var cacheAdminStoreType = typeof(CacheAdminStore<,>)
                        .MakeGenericType(adminStoreType.GetTypeInfo(), entityType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(cacheAdminStoreType);

                var iAdminStoreType = typeof(IAdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(iAdminStoreType, cacheAdminStoreType);
            }
        }
    }
}
