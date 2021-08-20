// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.Authentication.IntegrationTest
{
    public class TheIdServerTestFixture
    {
        /// <summary>
        /// Gets the system under test
        /// </summary>
        public TestServer Sut { get; }

        public TheIdServerTestFixture()
        {
            var dbName = Guid.NewGuid().ToString();
            Sut = TestUtils.CreateTestServer(
                services =>
                {
                    services.AddIdentityServer4AdminEntityFrameworkStores(options =>
                        options.UseInMemoryDatabase(dbName))
                    .AddIdentityProviderStore()
                    .AddConfigurationEntityFrameworkStores(options =>
                        options.UseInMemoryDatabase(dbName))
                    .AddOperationalEntityFrameworkStores(options =>
                        options.UseInMemoryDatabase(dbName))
                    .AddSingleton(provider =>
                        new OptionsMonitorCacheWrapper<CookieAuthenticationOptions>
                        (
                            provider.GetRequiredService<IOptionsMonitorCache<CookieAuthenticationOptions>>(),
                            provider.GetRequiredService<IEnumerable<IPostConfigureOptions<CookieAuthenticationOptions>>>(),
                            (name, configure) =>
                            {
                                
                            }
                        )
                    )
                    .AddSingleton(provider =>
                        new OptionsMonitorCacheWrapper<JwtBearerOptions>
                        (
                            provider.GetRequiredService<IOptionsMonitorCache<JwtBearerOptions>>(),
                            provider.GetRequiredService<IEnumerable<IPostConfigureOptions<JwtBearerOptions>>>(),
                            (name, configure) =>
                            {
                                configure.RequireHttpsMetadata = false;
                            }
                        )
                    )
                    .AddSingleton(provider =>
                        new OptionsMonitorCacheWrapper<WsFederationOptions>
                        (
                            provider.GetRequiredService<IOptionsMonitorCache<WsFederationOptions>>(),
                            provider.GetRequiredService<IEnumerable<IPostConfigureOptions<WsFederationOptions>>>(),
                            (name, configure) =>
                            {

                            }
                        )
                    );
                });

            using var scope = Sut.Host.Services.CreateScope();
            using var identityContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            identityContext.Database.EnsureCreated();
            using var operationalContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
            operationalContext.Database.EnsureCreated();
            using var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            appContext.Database.EnsureCreated();
        }
    }
}
