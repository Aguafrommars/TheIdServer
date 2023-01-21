// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Duende.IdentityServer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddClaimsProviders_should_not_throw_when_section_not_exists()
        {
            var configuration = new ConfigurationBuilder().Build();
            var services = new ServiceCollection();

            services.AddClaimsProviders(configuration);

            Assert.Empty(services);
        }

        [Fact]
        public void AddClaimsProviders_should_load_claims_provider_setup_from_assembly_path()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ClaimsProviderOptions:0:AssemblyPath"] = $"{typeof(ClaimsProvider).Assembly.GetName().Name}.dll",
                    ["ClaimsProviderOptions:0:TypeName"] = $"{typeof(ClaimsProviderSetup).FullName}"
                })
                .Build();
            var services = new ServiceCollection();

            services.AddClaimsProviders(configuration);

            Assert.NotEmpty(services);
        }

        class ClaimsProviderSetup : ISetupClaimsProvider
        {
            public IServiceCollection SetupClaimsProvider(IServiceCollection services, IConfiguration configuration)
            {
                return services.AddTransient<IProvideClaims, ClaimsProvider>();
            }
        }

        class ClaimsProvider : IProvideClaims
        {
            public Task<IEnumerable<Claim>> ProvideClaims(ClaimsPrincipal subject, Client client, string caller, Resource resource)
            {
                throw new NotImplementedException();
            }
        }
    }
}
