// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Admin.Configuration
{
    /// <summary>
    /// Signing key definition kinds
    /// </summary>
    public enum KeyKinds
    {
        /// <summary>
        /// From X509 file
        /// </summary>
        File,
        /// <summary>
        /// Temp key for development
        /// </summary>
        Development,
        /// <summary>
        /// From X509 store
        /// </summary>
        Store,
        /// <summary>
        /// From keys rotation
        /// </summary>
        KeysRotation
    }
}
