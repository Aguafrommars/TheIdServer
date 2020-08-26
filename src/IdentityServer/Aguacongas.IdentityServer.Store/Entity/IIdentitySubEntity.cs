// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Idenetity resource sub entity interface
    /// </summary>
    public interface IIdentitySubEntity
    {
        /// <summary>
        /// Gets or sets the identity resource identifier.
        /// </summary>
        /// <value>
        /// The API identifier.
        /// </value>
        string IdentityId { get; set; }
    }
}
