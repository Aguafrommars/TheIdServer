using Aguacongas.IdentityServer.Admin.Services.WindowsAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Aguacongas.IdentityServer.Duende.Test.Services.WindowsAuthentication.Extensions
{
    public class AuthenticationBuilderExtensions
    {
        [Fact]
        public void AddWindows_should_register_windows_authentication_service()
        {
            var builder = new ServiceCollection().AddAuthentication().AddWindows();
            Assert.Contains(builder.Services, s => s.ImplementationType == typeof(WindowsHandler));
            Assert.Contains(builder.Services, s => s.ImplementationType == typeof(PostConfigureWindowsOptions));
        }
    }
}
