// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.IdentityServer.KeysRotation
{
    /// <summary>
    /// Keys rotation builder interface
    /// </summary>
    public interface IKeyRotationBuilder
    {
        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        IServiceCollection Services { get; set; }
    }
}