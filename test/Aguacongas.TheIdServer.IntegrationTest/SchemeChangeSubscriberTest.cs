using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
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
        public async Task Subscribe_should_subcribe_to_hub_events()
        {
            var waitHandle = new ManualResetEvent(false);

            var configuration = new Dictionary<string, string>
            {
                ["ApiAuthentication:Authority"] = "http://localhost",
                ["SignalR:HubUrl"] = "http://localhost/providerhub",
                ["SignalR:UseMessagePack"] = "false"
            };
            var configurationMock = new Mock<IConfiguration>();
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

            Assert.True(waitHandle.WaitOne(5000));

            var provider = server.Host.Services;
            var store = provider.GetRequiredService<IAdminStore<ExternalProvider>>();
            var serializer = provider.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();

            var extProvider = new ExternalProvider
            {
                DisplayName = "Google",
                Id = "google",
                KindName = "Google",
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            };

            provider.GetRequiredService<IConfiguration>()["SignalR:HubUrl"] = null;

            await store.CreateAsync(extProvider).ConfigureAwait(false);
            await store.UpdateAsync(extProvider).ConfigureAwait(false);
            await store.DeleteAsync("google").ConfigureAwait(false);

            await Task.Delay(1000).ConfigureAwait(false);
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
            HttpClient _client;
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
