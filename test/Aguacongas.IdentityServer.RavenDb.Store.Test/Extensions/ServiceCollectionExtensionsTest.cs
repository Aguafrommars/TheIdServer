// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
            var services = new ServiceCollection();

            services.AddIdentityServer4AdminRavenDbStores();

            var assembly = typeof(Entity.IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                t.GetInterface(nameof(Entity.IEntityId)) != null);

            foreach(var entityType in entityTypeList)
            {
                var storeType = typeof(IAdminStore<>).MakeGenericType(entityType);
                Assert.Contains(services, d => d.ImplementationType != null && d.ImplementationType.GetInterfaces().Any(i => i == storeType));
            }
        }

        [Fact]
        public void AddIdentityServer4AdminRavenDbkStores_should_add_ravendb_stores_for_each_entity_using_getDocumentStore_function()
        {
            var services = new ServiceCollection();

            services.AddIdentityServer4AdminRavenDbStores(p => new RavenDbTestDriverWrapper().GetDocumentStore());

            var assembly = typeof(Entity.IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                t.GetInterface(nameof(Entity.IEntityId)) != null);

            foreach (var entityType in entityTypeList)
            {
                var storeType = typeof(IAdminStore<>).MakeGenericType(entityType);
                Assert.Contains(services, d => d.ServiceType == storeType);
            }
        }
    }
}
