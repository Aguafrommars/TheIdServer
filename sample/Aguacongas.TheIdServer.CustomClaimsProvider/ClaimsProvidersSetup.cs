// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Aguacongas.TheIdServer.CustomClaimsProviders
{
    public class ClaimsProvidersSetup : ISetupClaimsProvider
    {
        public IServiceCollection SetupClaimsProvider(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("claims")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(configuration.GetValue<string>("ClaimsWebServiceUrl")));

            return services.AddTransient<IProvideClaims>(p => new WebServiceClaimsProvider(p.GetRequiredService<IHttpClientFactory>().CreateClient("claims")));
        }
    }
}
