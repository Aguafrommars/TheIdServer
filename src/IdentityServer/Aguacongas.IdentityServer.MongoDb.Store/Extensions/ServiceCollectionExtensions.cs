// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.MongoDb.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServer4AdminMongoDbStores(this IServiceCollection services, string connectionString)
        {
            var uri = new Uri(connectionString);
            return services
                .AddScoped<IMongoClient>(p => new MongoClient(connectionString))
                .AddScoped(p => p.GetRequiredService<IMongoClient>().GetDatabase(uri.Segments[1]))
                .AddIdentityServer4AdminMongoDbStores(getDatabase: null);
        }

        public static IServiceCollection AddIdentityServer4AdminMongoDbStores(this IServiceCollection services, Func<IServiceProvider, IMongoDatabase> getDatabase)
        {
            if (getDatabase == null)
            {
                getDatabase = p => p.GetRequiredService<IMongoDatabase>();
            }

            var entityTypeList = GetEntityTypes();

            foreach (var entityType in entityTypeList)
            {
                AddMonogoAdminStore(services, entityType, getDatabase);
            }

            return services;
        }

        private static void AddMonogoAdminStore(IServiceCollection services, Type entityType, Func<IServiceProvider, IMongoDatabase> getDatabase)
        {
            services.AddScoped(typeof(IMongoCollection<>).MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo(), provider =>
            {
                return GetCollection(getDatabase, provider, entityType);
            });
            services.AddTransient(typeof(IAdminStore<>).MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo(), 
                typeof(AdminStore<>).MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo());
        }

        private static object GetCollection(Func<IServiceProvider, IMongoDatabase> getDatabase, IServiceProvider provider, Type entityType)
        {
            var database = getDatabase(provider);
            return database.GetCollection(entityType);
        }

        private static IEnumerable<Type> GetEntityTypes()
        {
            var assembly = typeof(IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract &&
                t.GetInterface("IEntityId") != null);
            return entityTypeList;
        }
    }
}
