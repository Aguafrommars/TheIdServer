using Aguacongas.IdentityServer.Admin.Services.WindowsAuthentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Shared.Services.WindowsAuthentication
{
    public class PostConfigureWindowsOptionsTest
    {
        [Fact]
        public void PostConfigure_should_enable_ldap()
        {
            var sut = new PostConfigureWindowsOptions(Array.Empty<IServerIntegratedAuth>(), 
                new NullLogger<NegotiateHandler>());
            sut.PostConfigure(null, new WindowsOptions
            {
                LdapEnabled = true
            });
            sut.PostConfigure(null, new WindowsOptions
            {
                LdapEnabled = false
            });
        }
    }
}
