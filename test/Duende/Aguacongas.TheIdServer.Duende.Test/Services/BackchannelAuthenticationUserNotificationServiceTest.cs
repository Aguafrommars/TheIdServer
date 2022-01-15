using Aguacongas.IdentityServer.Services;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Duende.Test.Services
{
    public class BackchannelAuthenticationUserNotificationServiceTest
    {
        [Fact]
        public void Constructor_shoult_validate_arguments()
        {
            Assert.Throws<ArgumentNullException>(() => new BackchannelAuthenticationUserNotificationService(null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new BackchannelAuthenticationUserNotificationService(new Mock<IIssuerNameService>().Object, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new BackchannelAuthenticationUserNotificationService(new Mock<IIssuerNameService>().Object, 
                new Mock<IStringLocalizer<BackchannelAuthenticationUserNotificationService>>().Object, null, null));
            Assert.Throws<ArgumentNullException>(() => new BackchannelAuthenticationUserNotificationService(new Mock<IIssuerNameService>().Object,
                new Mock<IStringLocalizer<BackchannelAuthenticationUserNotificationService>>().Object, new HttpClient(), null));
        }

        [Theory]
        [InlineData(null, null, "https://theidserver")]
        [InlineData("https://theidserver", "secret", "https://theidserver/")]
        public async Task SendLoginRequestAsync_should_call_email_service(string logouri, string bindinMessage, string issuer)
        {
            var mockHttpHandler = new MockHttpMessageHandler();
            mockHttpHandler.When(HttpMethod.Post, "https://theidserver/email").Respond(r =>
            {
                Assert.Equal("https://theidserver/email", r.RequestUri.ToString());
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            var client = mockHttpHandler.ToHttpClient();
            var issuerNameServiceMock = new Mock<IIssuerNameService>();
            issuerNameServiceMock.Setup(m => m.GetCurrentAsync()).ReturnsAsync(issuer);

            var stringLocalizerMock = new Mock<IStringLocalizer<BackchannelAuthenticationUserNotificationService>>();
            var options = Options.Create(new BackchannelAuthenticationUserNotificationServiceOptions
            {
                ApiUrl = "https://theidserver/email"
            });
            var sut = new BackchannelAuthenticationUserNotificationService(issuerNameServiceMock.Object, stringLocalizerMock.Object, client, options);

            await sut.SendLoginRequestAsync(new BackchannelUserLoginRequest
            {
                Client = new Client
                {
                    LogoUri = logouri,
                    ClientName = Guid.NewGuid().ToString(), 
                },
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new [] { new Claim(JwtClaimTypes.Email, "aguacongas@gmail.com") })),
                InternalId = Guid.NewGuid().ToString(),
            }).ConfigureAwait(false);
        }
    }
}
