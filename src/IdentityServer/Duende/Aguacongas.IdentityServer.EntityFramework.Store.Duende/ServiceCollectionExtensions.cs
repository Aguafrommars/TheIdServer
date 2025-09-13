// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using Aguacongas.IdentityServer.Extensions;
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
        public static IServiceCollection AddTheIdServerAdminEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            AddStoresForContext(services, typeof(ApplicationDbContext));
            return services.AddDbContext<ApplicationDbContext>(optionsAction);
        }

        public static IServiceCollection AddConfigurationEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            AddStoresForContext(services, typeof(ConfigurationDbContext));
            return services.AddDbContext<ConfigurationDbContext>(optionsAction);
        }

        public static IServiceCollection AddOperationalEntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            AddStoresForContext(services, typeof(OperationalDbContext));
            return services.AddDbContext<OperationalDbContext>(optionsAction);
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
