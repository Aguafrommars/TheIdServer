﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Raven.Client.Documents;
using System.Linq;
using System.Reflection;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddIdentityServer4AdminRavenDbkStores_should_add_ravendb_stores_for_each_entity()
        {
            var services = new ServiceCollection().AddLogging();
            
            var wrapper = new RavenDbTestDriverWrapper();
            services.AddIdentityServer4AdminRavenDbStores()
                .AddSingleton<HubConnectionFactory>()
                .AddTransient(p => new Mock<IConfiguration>().Object)
                .AddTransient<IProviderClient, ProviderClient>()
                .AddTransient(p => wrapper.GetDocumentStore());

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores();

            services.AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddTheIdServerStoreRavenDbStore()
                .AddGoogle();                

            var assembly = typeof(Entity.IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => 
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                t.GetInterface(nameof(Entity.IEntityId)) != null);

            var provider = services.BuildServiceProvider();
            foreach(var entityType in entityTypeList)
            {
                var storeType = typeof(IAdminStore<>).MakeGenericType(entityType);
                Assert.NotNull(provider.GetService(storeType));
            }
        }

        [Fact]
        public void AddIdentityServer4AdminRavenDbkStores_should_add_ravendb_stores_for_each_entity_using_getDocumentStore_function()
        {
            var services = new ServiceCollection().AddLogging();

            var wrapper = new RavenDbTestDriverWrapper();
            services.AddIdentityServer4AdminRavenDbStores(p => new RavenDbTestDriverWrapper().GetDocumentStore())
                .AddSingleton<HubConnectionFactory>()
                .AddTransient(p => new Mock<IConfiguration>().Object)
                .AddTransient<IProviderClient, ProviderClient>()
                .AddTransient(p => wrapper.GetDocumentStore());

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores();

            services.AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddTheIdServerStoreRavenDbStore()
                .AddGoogle();

            var assembly = typeof(Entity.IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
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
