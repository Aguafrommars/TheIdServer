using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components.Testing
{
    public static class MockHttpExtensions
    {
        public static JsonSerializerOptions DefaultJsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static MockHttpMessageHandler AddMockHttp(this TestHost host, string baseUri = "http://example.com")
        {
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri(baseUri);
            host.AddService(httpClient);
            return mockHttp;
        }

        public static TaskCompletionSource<object> Capture(this MockHttpMessageHandler handler,
            string url,
            JsonSerializerOptions serializationSettings = null)
        {
            var tcs = new TaskCompletionSource<object>();

            handler.When(url).Respond(() =>
            {
                return tcs.Task.ContinueWith(task =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonSerializer.Serialize(task.Result, serializationSettings ?? DefaultJsonSerializerOptions))
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    return response;
                });
            });

            return tcs;
        }
    }
}
