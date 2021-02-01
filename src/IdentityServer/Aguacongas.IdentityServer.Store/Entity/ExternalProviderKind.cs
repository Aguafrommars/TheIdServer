// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Supported external provider lind
    /// </summary>
    public class ExternalProviderKind
    {
        /// <summary>
        /// Gets or sets the kind of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        public string KindName { get; set; }

        /// <summary>
        /// Gets or sets the type of the serialized handler.
        /// </summary>
        /// <value>
        /// The type of the serialized handler.
        /// </value>
        public string SerializedHandlerType { get; set; }

        /// <summary>
        /// Gets or sets the serialized default options.
        /// </summary>
        /// <value>
        /// The serialized default options.
        /// </value>
        public string SerializedDefaultOptions { get; set; }
    }
}
