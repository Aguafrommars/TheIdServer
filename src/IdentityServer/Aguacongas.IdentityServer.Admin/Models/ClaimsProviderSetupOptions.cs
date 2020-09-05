// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Admin.Models
{
    /// <summary>
    /// Define a claims provider setup class options
    /// </summary>
    public class ClaimsProviderSetupOptions
    {
        /// <summary>
        /// Gets or sets the assembly path.
        /// </summary>
        /// <value>
        /// The assembly path.
        /// </value>
        public string AssemblyPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName { get; set; }
    }
}
