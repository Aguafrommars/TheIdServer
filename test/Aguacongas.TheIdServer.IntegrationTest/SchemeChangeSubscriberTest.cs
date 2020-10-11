// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.AspNetCore.Authentication.EntityFramework;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Http.Store;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public class SchemeChangeSubscriberTest
    {
        [Fact]
        public void Constructor_should_throw_on_args_null()
        {
            Assert.Throws<ArgumentNullException>(() => new SchemeChangeSubscriber<SchemeDefinition>(null, null, null, null, null, null));
            var hubConnectionFactory = new HubConnectionFactory(new Mock<IConfiguration>().Object, new Mock<IServiceProvider>().Object, new NullLogger<HubConnectionFactory>());
            Assert.Throws<ArgumentNullException>(() => new SchemeChangeSubscriber<SchemeDefinition>(hubConnectionFactory, null, null, null, null, null));
            var manager = new NoPersistentDynamicManager<SchemeDefinition>(new Mock<IAuthenticationSchemeProvider>().Object, new OptionsMonitorCacheWrapperFactory(new Mock<IServiceProvider>().Object), Array.Empty<Type>());
            Assert.Throws<ArgumentNullException>(() => new SchemeChangeSubscriber<SchemeDefinition>(hubConnectionFactory, manager, null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new SchemeChangeSubscriber<SchemeDefinition>(hubConnectionFactory, manager, new Mock<IDynamicProviderStore<SchemeDefinition>>().Object, null, null, null));
            var wrapper1 = new KeyManagerWrapper<Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.IAuthenticatedEncryptorDescriptor>(new Mock<IKeyManager>().Object, new Mock<IDefaultKeyResolver>().Object, new Mock<IProviderClient>().Object);
            Assert.Throws<ArgumentNullException>(() => new SchemeChangeSubscriber<SchemeDefinition>(hubConnectionFactory, manager, new Mock<IDynamicProviderStore<SchemeDefinition>>().Object, wrapper1, null, null));
            var wrapper2 = new KeyManagerWrapper<IdentityServer.KeysRotation.RsaEncryptorDescriptor>(new Mock<IKeyManager>().Object, new Mock<IDefaultKeyResolver>().Object, new Mock<IProviderClient>().Object);
            Assert.Throws<ArgumentNullException>(() => new SchemeChangeSubscriber<SchemeDefinition>(hubConnectionFactory, manager, new Mock<IDynamicProviderStore<SchemeDefinition>>().Object, wrapper1, wrapper2, null));
        }

        [Fact]
        public async Task Subscribe_should_subcribe_to_hub_events()
        {
            var waitHandle = new ManualResetEvent(false);

            var configuration = new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = Guid.NewGuid().ToString(),
                ["ApiAuthentication:Authority"] = "http://localhost",
                ["SignalR:HubUrl"] = "http://localhost/providerhub",
                ["SignalR:UseMessagePack"] = "false",
                ["Seed"] = "true"
            };

            TestServer server = null;
            server = TestUtils.CreateTestServer(services =>
            {
                services.RemoveAll<HubHttpMessageHandlerAccessor>();
                services.AddTransient(p => new HubHttpMessageHandlerAccessor { Handler = new MockHttpMessageHandler(waitHandle, server.CreateHandler()) });
                services.RemoveAll<HttpClient>();
                services.AddTransient(p => server.CreateClient());
                services.RemoveAll<HttpClientHandler>();
                services.AddTransient<HttpClientHandler>(p => new MockHttpClientHandler(p.GetRequiredService<HttpClient>()));
            }, configuration);

            server.CreateWebSocketClient();

            var provider = server.Host.Services;
            var store = provider.GetRequiredService<IAdminStore<ExternalProvider>>();
            var serializer = provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();

            var manager = new NoPersistentDynamicManager<SchemeDefinition>(new Mock<IAuthenticationSchemeProvider>().Object, new OptionsMonitorCacheWrapperFactory(new Mock<IServiceProvider>().Object), Array.Empty<Type>());
            var wrapper1 = new KeyManagerWrapper<Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.IAuthenticatedEncryptorDescriptor>(new Mock<IKeyManager>().Object, new Mock<IDefaultKeyResolver>().Object, new Mock<IProviderClient>().Object);
            var wrapper2 = new KeyManagerWrapper<IdentityServer.KeysRotation.RsaEncryptorDescriptor>(new Mock<IKeyManager>().Object, new Mock<IDefaultKeyResolver>().Object, new Mock<IProviderClient>().Object);
            var hubConnectionFactory = new HubConnectionFactory(provider.GetRequiredService<IConfiguration>(), provider, new NullLogger<HubConnectionFactory>());

            var connection = hubConnectionFactory.GetConnection(default);
            Assert.NotNull(connection);
            await hubConnectionFactory.StartConnectionAsync(default);

            var subscriber = new SchemeChangeSubscriber<SchemeDefinition>(hubConnectionFactory, manager, new Mock<IDynamicProviderStore<SchemeDefinition>>().Object, wrapper1, wrapper2, new NullLogger<SchemeChangeSubscriber<SchemeDefinition>>());
            await subscriber.SubscribeAsync(default).ConfigureAwait(false);

            var extProvider = new ExternalProvider
            {
                DisplayName = "Google",
                Id = "google",
                KindName = "Google",
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            };

            var result = waitHandle.WaitOne(5000);

            Assert.True(result);

            await store.CreateAsync(extProvider).ConfigureAwait(false);
            await store.UpdateAsync(extProvider).ConfigureAwait(false);
            await store.DeleteAsync("google").ConfigureAwait(false);

            var providerClient = provider.GetRequiredService<IProviderClient>();

            await providerClient.KeyRevokedAsync(nameof(IAuthenticatedEncryptorDescriptor), Guid.NewGuid().ToString()).ConfigureAwait(false);
            await providerClient.KeyRevokedAsync(nameof(RsaEncryptorDescriptor), Guid.NewGuid().ToString()).ConfigureAwait(false);

            provider.GetRequiredService<IConfiguration>()["SignalR:HubUrl"] = null;

            await store.CreateAsync(extProvider).ConfigureAwait(false);
            await store.UpdateAsync(extProvider).ConfigureAwait(false);
            await store.DeleteAsync("google").ConfigureAwait(false);

            await providerClient.KeyRevokedAsync("test", "test").ConfigureAwait(false);
        }

        class MockHttpMessageHandler : DelegatingHandler
        {
            public MockHttpMessageHandler(ManualResetEvent waitHandle, HttpMessageHandler innerHandler) : base(innerHandler)
            {
                WaitHandle = waitHandle;
            }

            public ManualResetEvent WaitHandle { get; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var requestUri = request.RequestUri.PathAndQuery;

                var response = base.SendAsync(request, cancellationToken);

                if (requestUri.StartsWith("/providerhub?id="))
                {
                    WaitHandle.Set();
                }

                return response;
            }
        }

        class MockHttpClientHandler : HttpClientHandler
        {
            private readonly HttpClient _client;
            public MockHttpClientHandler(HttpClient client)
            {
                _client = client;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var newRequest = new HttpRequestMessage
                {
                    Content = request.Content,
                    Method = request.Method,
                    RequestUri = request.RequestUri
                };

                foreach(var header in request.Headers)
                {
                    newRequest.Headers.Add(header.Key, header.Value);
                }

                return _client.SendAsync(newRequest, cancellationToken);
            }
        }
    }
}
