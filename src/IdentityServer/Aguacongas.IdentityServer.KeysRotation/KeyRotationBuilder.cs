// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.IdentityServer.KeysRotation
{
    /// <summary>
    /// Keys rotation builder
    /// </summary>
    /// <seealso cref="IKeyRotationBuilder" />
    internal class KeyRotationBuilder : IKeyRotationBuilder
    {
        public IServiceCollection Services { get; set; }
    }
}
