// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class OAuthDelegatingHandlerTest
    {
        [Fact]
        public async Task SendAsync_should_set_request_authorization()
        {
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();

            var discoveryRequest = Capture(mockHttp, "https://exemple.com/.well-known/openid-configuration");
            var jwksRequest = Capture(mockHttp, "https://exemple.com/jwks");
            var tokenRequest = Capture(mockHttp, "https://exemple.com/token");
            mockHttp.When("http://test")
                .WithHeaders("Authorization", "test test")
                .Respond(new StringContent("succeed"));

            discoveryRequest.SetResult(new
            {
                issuer = "https://exemple.com",
                token_endpoint = "https://exemple.com/token",
                jwks_uri = "https://exemple.com/jwks"
            });

            tokenRequest.SetResult(new
            {
                access_token = "test",
                token_type = "test"
            });

            jwksRequest.SetResult(new
            {

            });

            var optionsMock = new Mock<IOptions<IdentityServerOptions>>();
            optionsMock.SetupGet(m => m.Value).Returns(new IdentityServerOptions
            {
                Authority = "https://exemple.com"
            });

            using var tokenManager = new OAuthTokenManager(httpClient, optionsMock.Object);

            var sut = new OAuthDelegatingHandler(tokenManager, mockHttp);

            var client = new HttpClient(sut);

            var response = await client.GetAsync("http://test");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal("succeed", content);
        }

        public static TaskCompletionSource<object> Capture(MockHttpMessageHandler handler,
            string url)
        {
            var tcs = new TaskCompletionSource<object>();
            handler.When(url).Respond(() =>
            {
                return tcs.Task.ContinueWith(task =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonSerializer.Serialize(task.Result, DefaultJsonSerializerOptions))
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    return response;
                });
            });

            return tcs;
        }

        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
