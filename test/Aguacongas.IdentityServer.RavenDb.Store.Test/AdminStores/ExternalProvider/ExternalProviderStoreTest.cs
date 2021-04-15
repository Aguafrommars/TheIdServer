// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test
{
    public class ExternalProviderStoreTest
    {
        [Fact]
        public void Constuctor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new ExternalProviderStore(null, null, null, null));
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            var provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();

            Assert.Throws<ArgumentNullException>(() => new ExternalProviderStore(manager,
                null, null, null));
            Assert.Throws<ArgumentNullException>(() => new ExternalProviderStore(manager,
                provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>(),
                null, null));
        }

        [Fact]
        public async Task CreateAsync_should_return_scheme_id()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();
            var serializer = provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            
            var sut = new ExternalProviderStore(manager,
                serializer,
                new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()),
                null);

            var result = await sut.CreateAsync(new Entity.ExternalProvider
            {
                DisplayName = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString(),
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            } as object);


            Assert.NotNull(result);
            Assert.NotNull(((Entity.ExternalProvider)result).Id);
        }

        [Fact]
        public async Task CreateAsync_should_return_notify_provider_added()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();
            var serializer = provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            var mockProviderClient = new Mock<IProviderClient>();
            mockProviderClient.Setup(m => m.ProviderAddedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();

            var sut = new ExternalProviderStore(manager,
                serializer,
                new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()),
                mockProviderClient.Object);

            await sut.CreateAsync(new Entity.ExternalProvider
            {
                DisplayName = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString(),
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            } as object);


            mockProviderClient.Verify();
        }

        [Fact]
        public async Task DeleteAsync_should_delete_scheme()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();
            var serializer = provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();

            var id = Guid.NewGuid().ToString();
            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = id
            });

            var sut = new ExternalProviderStore(manager,
                serializer,
                new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()),
                null);

            await sut.DeleteAsync(id);

            var scheme = await manager.FindBySchemeAsync(id);
            Assert.Null(scheme);
        }

        [Fact]
        public async Task DeleteAsync_should_notity_provider_deleted()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();
            var serializer = provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            var mockProviderClient = new Mock<IProviderClient>();
            mockProviderClient.Setup(m => m.ProviderRemovedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();

            var id = Guid.NewGuid().ToString();
            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = id
            });

            var sut = new ExternalProviderStore(manager,
                serializer,
                new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()),
                mockProviderClient.Object);

            await sut.DeleteAsync(id);

            mockProviderClient.Verify();
        }

        [Fact]
        public async Task UdpateAsync_should_update_scheme()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();
            var serializer = provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            

            var id = Guid.NewGuid().ToString();
            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = id
            });

            var sut = new ExternalProviderStore(manager,
                serializer,
                new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()),
                null);

            var expected = Guid.NewGuid().ToString();
            await sut.UpdateAsync(new Entity.ExternalProvider
            {
                DisplayName = expected,
                Id = id,
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            } as object);

            var scheme = await manager.FindBySchemeAsync(id);
            Assert.Equal(expected, scheme.DisplayName);
        }

        [Fact]
        public async Task UdpateAsync_should_notify_provider_updated()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();
            var serializer = provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            var mockProviderClient = new Mock<IProviderClient>();
            mockProviderClient.Setup(m => m.ProviderUpdatedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();

            var id = Guid.NewGuid().ToString();
            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = id
            });

            var sut = new ExternalProviderStore(manager,
                serializer,
                new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()),
                mockProviderClient.Object);

            var expected = Guid.NewGuid().ToString();
            await sut.UpdateAsync(new Entity.ExternalProvider
            {
                DisplayName = expected,
                Id = id,
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            } as object);

            mockProviderClient.Verify();
        }

        [Fact]
        public async Task GetAsync_by_page_request_should_find_schemes()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();

            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = $"scheme-{Guid.NewGuid()}"
            });

            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = $"scheme-{Guid.NewGuid()}"
            });

            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = $"scheme-{Guid.NewGuid()}"
            });

            var sut = new ExternalProviderStore(manager,
                provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>(),
                new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()),
                provider.GetService<IProviderClient>());

            var rolesResult = await sut.GetAsync(new PageRequest
            {
                Filter = $"contains(Scheme, 'scheme')",
                Take = 1
            });

            Assert.NotNull(rolesResult);
            Assert.Equal(3, rolesResult.Count);
            Assert.Single(rolesResult.Items);
        }

        [Fact]
        public async Task GetAsync_by_id_should_find_scheme()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();

            var id = Guid.NewGuid().ToString();
            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = id
            });

            var sut = new ExternalProviderStore(manager,
                provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>(),
                new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()),
                provider.GetService<IProviderClient>());

            var result = await sut.GetAsync(id, null);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_by_id_should_expand_transforms()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();

            var id = Guid.NewGuid().ToString();
            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = id
            });

            using var session = documentStore.OpenAsyncSession();
            var transformId = Guid.NewGuid().ToString();
            await session.StoreAsync(new Entity.ExternalClaimTransformation
            {
                Scheme = id,
                Id = transformId,
                FromClaimType = "test",
                ToClaimType = "test"
            }, $"{nameof(Entity.ExternalClaimTransformation).ToLowerInvariant()}/{transformId}").ConfigureAwait(false);
            var saved = await session.LoadAsync<SchemeDefinition>($"{nameof(SchemeDefinition).ToLowerInvariant()}/{id}").ConfigureAwait(false);
            saved.ClaimTransformations = new List<Entity.ExternalClaimTransformation>
            {
                new Entity.ExternalClaimTransformation
                {
                    Id = $"{nameof(Entity.ExternalClaimTransformation).ToLowerInvariant()}/{transformId}"
                }
            };
            await session.SaveChangesAsync().ConfigureAwait(false);

            var sut = new ExternalProviderStore(manager,
                provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>(),
                new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()),
                provider.GetService<IProviderClient>());

            var result = await sut.GetAsync(id, new GetRequest
            {
                Expand = nameof(SchemeDefinition.ClaimTransformations)
            });

            Assert.NotNull(result);
            Assert.Single(result.ClaimTransformations);
            Assert.Equal("test", result.ClaimTransformations.First().FromClaimType);
        }
    }
}
