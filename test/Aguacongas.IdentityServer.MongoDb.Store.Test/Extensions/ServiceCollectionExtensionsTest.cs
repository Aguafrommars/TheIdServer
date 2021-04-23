// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.MongoDb.Store.Test.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddIdentityServer4AdminMongoDbkStores_with_connectionString_should_add_ravendb_stores_for_each_entity()
        {
            var services = new ServiceCollection().AddLogging();

            services.AddIdentityServer4AdminMongoDbStores("mongodb://localhost/test")
                .AddSingleton<HubConnectionFactory>()
                .AddTransient(p => new Mock<IConfiguration>().Object)
                .AddTransient<IProviderClient, ProviderClient>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores();

            services.AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddTheIdServerEntityMongoDbStore()
                .AddGoogle();

            var assembly = typeof(Entity.IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                t.GetInterface(nameof(Entity.IEntityId)) != null);

            var provider = services.BuildServiceProvider();
            foreach (var entityType in entityTypeList)
            {
                var storeType = typeof(IAdminStore<>).MakeGenericType(entityType);
                Assert.NotNull(provider.GetService(storeType));
            }
        }

        [Fact]
        public void AddIdentityServer4AdminMongoDbkStores_with_getDatabase_should_add_ravendb_stores_for_each_entity()
        {
            var services = new ServiceCollection().AddLogging();

            var connectionString = "mongodb://localhost/test";
            var uri = new Uri(connectionString);
            services.AddIdentityServer4AdminMongoDbStores(p => p.GetRequiredService<IMongoDatabase>())
                .AddScoped<IMongoClient>(p => new MongoClient(connectionString))
                .AddScoped(p => p.GetRequiredService<IMongoClient>().GetDatabase(uri.Segments[1]))
                .AddSingleton<HubConnectionFactory>()
                .AddTransient(p => new Mock<IConfiguration>().Object)
                .AddTransient<IProviderClient, ProviderClient>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores();

            services.AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddTheIdServerEntityMongoDbStore()
                .AddGoogle();

            var assembly = typeof(Entity.IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                t.GetInterface(nameof(Entity.IEntityId)) != null);

            var provider = services.BuildServiceProvider();
            foreach (var entityType in entityTypeList)
            {
                var storeType = typeof(IAdminStore<>).MakeGenericType(entityType);
                Assert.NotNull(provider.GetService(storeType));
            }
        }
    }
}
