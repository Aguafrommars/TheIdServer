// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System.Security.Cryptography.X509Certificates;

namespace Aguacongas.IdentityServer.Admin.Configuration
{
    /// <summary>
    /// Signing key definition
    /// </summary>
    public class KeyDefinition
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public KeyKinds? Type { get; set; }
        /// <summary>
        /// Gets or sets the persisted.
        /// </summary>
        /// <value>
        /// The persisted.
        /// </value>
        public bool? Persisted { get; set; }
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the store location.
        /// </summary>
        /// <value>
        /// The store location.
        /// </value>
        public StoreLocation? StoreLocation { get; set; }
        /// <summary>
        /// Gets or sets the name of the store.
        /// </summary>
        /// <value>
        /// The name of the store.
        /// </value>
        public string StoreName { get; set; }
        /// <summary>
        /// Gets or sets the storage flags.
        /// </summary>
        /// <value>
        /// The storage flags.
        /// </value>
        public string StorageFlags { get; set; }
    }
}
