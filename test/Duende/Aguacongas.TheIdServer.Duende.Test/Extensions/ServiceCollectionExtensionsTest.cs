using Aguacongas.IdentityServer.Services;
using Duende.IdentityServer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Duende.Test.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddCibaServices_shoul_load_type_assembly_in_config()
        {
            var configuration = new ConfigurationManager().AddInMemoryCollection(new Dictionary<string, string>
            {
                [$"{nameof(BackchannelAuthenticationUserNotificationServiceOptions.AssemblyPath)}"] = typeof(BackchannelAuthenticationUserNotificationService).Assembly.Location,
                [$"{nameof(BackchannelAuthenticationUserNotificationServiceOptions.ServiceType)}"] = typeof(BackchannelAuthenticationUserNotificationService).FullName
            }).Build();
            var services = new ServiceCollection();
            services.AddIdentityServer()
                .AddCiba(configuration);
            var provider = services.AddLocalization()
                .BuildServiceProvider();

            var result = provider.GetService<IBackchannelAuthenticationUserNotificationService>();
            Assert.NotNull(result);
        }
    }
}
