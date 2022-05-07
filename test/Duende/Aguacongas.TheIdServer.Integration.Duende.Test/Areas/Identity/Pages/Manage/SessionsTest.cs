using Aguacongas.TheIdServer.IntegrationTest;
using Aguacongas.TheIdServer.IntegrationTest.BlazorApp;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Integration.Duende.Test.Areas.Identity.Pages.Manage
{
    [Collection(BlazorAppCollection.Name)]
    public class SessionsTest
    {
        private TheIdServerFactory _factory;
        public SessionsTest(TheIdServerFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task OnGetAsync_should_read_user_session()
        {
            var subjectId = Guid.NewGuid().ToString();
            var testUserService = _factory.Services.GetRequiredService<TestUserService>();
            testUserService.SetTestUser(true, new[] { new Claim("sub", subjectId) });

            using var client1 = _factory.CreateClient();
            using var response1 = await client1.GetAsync("Identity/Account/Manage/Sessions");

            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            var content1 = await response1.Content.ReadAsStringAsync();

            Assert.Contains("No session", content1);

            using var scope = _factory.Services.CreateScope();
            var ticketService = scope.ServiceProvider.GetRequiredService<IServerSideTicketStore>();
            await ticketService.StoreAsync(new AuthenticationTicket(testUserService.User ?? throw new Exception(),
                new AuthenticationProperties(new Dictionary<string, string?>
                {
                    ["session_id"] = Guid.NewGuid().ToString()
                }), 
                "Bearer"));

            using var client2 = _factory.CreateClient();
            using var response2 = await client2.GetAsync("Identity/Account/Manage/Sessions");

            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            var content2 = await response2.Content.ReadAsStringAsync();

            Assert.DoesNotContain("No session", content2);
        }

        [Fact]
        public async Task OnPostDeleteAsync_should_remove_user_session()
        {
            var subjectId = Guid.NewGuid().ToString();
            var testUserService = _factory.Services.GetRequiredService<TestUserService>();
            testUserService.SetTestUser(true, new[] { new Claim("sub", subjectId) });

            using var scope = _factory.Services.CreateScope();
            var ticketService = scope.ServiceProvider.GetRequiredService<IServerSideTicketStore>();
            await ticketService.StoreAsync(new AuthenticationTicket(testUserService.User ?? throw new Exception(),
                new AuthenticationProperties(new Dictionary<string, string?>
                {
                    ["session_id"] = Guid.NewGuid().ToString()
                }),
                "Bearer"));

            using var client = _factory.CreateClient();
            using var response1 = await client.GetAsync("Identity/Account/Manage/Sessions");

            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            var content1 = await response1.Content.ReadAsStringAsync();

            var token = GetFormValue(content1, "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"");
            var sessionId = GetFormValue(content1, "<input type=\"hidden\" name=\"sessionId\" value=\"");

            using var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["sessionId"] = sessionId,
                ["__RequestVerificationToken"] = token
            });
            
            using var response2 = await client.PostAsync("Identity/Account/Manage/Sessions?handler=Delete", formContent);

            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            var content2 = await response2.Content.ReadAsStringAsync();

            Assert.Contains("No session", content2);
        }

        private static string GetFormValue(string content, string match)
        {
            var startIndex = content.IndexOf(match);
            startIndex += match.Length;
            var endIndex = content.IndexOf('"', startIndex);
            return content[startIndex..endIndex];
        }
    }
}

