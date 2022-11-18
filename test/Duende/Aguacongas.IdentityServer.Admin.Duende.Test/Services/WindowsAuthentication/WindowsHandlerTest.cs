using Aguacongas.IdentityServer.Admin.Services.WindowsAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Duende.Test.Services.WindowsAuthentication
{
    public class WindowsHandlerTest
    {
        [Fact]
        public async Task AuthenticateAsync_should_be_delegated_to_inner_handler()
        {
            var sut = await InitializeSut();
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AuthenticateAsync());
        }

        [Fact]
        public async Task ChallengeAsync_should_be_delegated_to_inner_handler()
        {
            var sut = await InitializeSut();
            await Assert.ThrowsAsync<NullReferenceException>(() => sut.ChallengeAsync(new AuthenticationProperties()));
        }

        [Fact]
        public async Task ForbidAsync_should_be_delegated_to_inner_handler()
        {
            var sut = await InitializeSut();
            await Assert.ThrowsAsync<NullReferenceException>(() => sut.ForbidAsync(new AuthenticationProperties()));
        }

        private static async Task<WindowsHandler> InitializeSut()
        {
            var settings = new WindowsOptions();
            var optionsMonitorMock = new Mock<IOptionsMonitor<WindowsOptions>>();
            optionsMonitorMock.Setup(m => m.Get(It.IsAny<string>())).Returns(settings);

            var loggerFactoryMock = new Mock<ILoggerFactory>();
            var urlEncoderMock = new Mock<UrlEncoder>();
            var systemClockMock = new Mock<ISystemClock>();

            var sut = new WindowsHandler(optionsMonitorMock.Object,
                loggerFactoryMock.Object,
                urlEncoderMock.Object,
                systemClockMock.Object);

            var httpRequestMock = new Mock<HttpRequest>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(m => m.Request).Returns(httpRequestMock.Object);
            httpRequestMock.SetupGet(m => m.HttpContext).Returns(httpContextMock.Object);

            await sut.InitializeAsync(new AuthenticationScheme("Windows", "Windows", typeof(WindowsHandler)),
                httpContextMock.Object);
            return sut;
        }
    }
}
