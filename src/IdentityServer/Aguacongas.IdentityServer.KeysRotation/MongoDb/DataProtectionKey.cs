// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.KeysRotation.MongoDb
{
    /// <summary>
    /// Code first model used by <see cref="EntityFrameworkCoreXmlRepository{TContext}"/>.
    /// </summary>
    public class DataProtectionKey : IXmlKey
    {
        /// <summary>
        /// The entity identifier of the <see cref="KeyRotationKey"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The friendly name of the <see cref="KeyRotationKey"/>.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The XML representation of the <see cref="KeyRotationKey"/>.
        /// </summary>
        public string Xml { get; set; }
    }
}
