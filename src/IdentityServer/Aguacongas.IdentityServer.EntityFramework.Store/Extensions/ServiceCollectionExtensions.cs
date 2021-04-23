// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Identity;
using Aguacongas.TheIdServer.Models;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Authentication;
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
                    var userStore = new AdminStore<User, ApplicationDbContext>(
                        p.GetRequiredService<ApplicationDbContext>(),
                        p.GetRequiredService<ILogger<AdminStore<User, ApplicationDbContext>>>());
                    var roleStore = new AdminStore<Role, ApplicationDbContext>(
                        p.GetRequiredService<ApplicationDbContext>(),
                        p.GetRequiredService<ILogger<AdminStore<Role, ApplicationDbContext>>>());

                    return new CheckIdentityRulesUserStore(userStore,
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
                    var store = new AdminStore<Role, ApplicationDbContext>(
                        p.GetRequiredService<ApplicationDbContext>(),
                        p.GetRequiredService<ILogger<AdminStore<Role, ApplicationDbContext>>>());

                    return new CheckIdentityRulesRoleStore(store,
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
                var iAdminStoreType = typeof(IAdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                services.AddTransient(iAdminStoreType, adminStoreType);
            }
        }
    }
}
