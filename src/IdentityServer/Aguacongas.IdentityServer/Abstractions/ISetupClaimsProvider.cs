// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface ISetupClaimsProvider
    {
        IServiceCollection SetupClaimsProvider(IServiceCollection services, IConfiguration configuration);
    }
}
