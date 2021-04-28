// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    public interface IMetadataResponseGenerator
    {
        Task<WsFederationConfiguration> GenerateAsync(string wsfedEndpoint);
    }
}