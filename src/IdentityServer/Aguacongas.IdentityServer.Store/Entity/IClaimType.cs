// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Classes implementing this interface define claim type
    /// </summary>
    public interface IClaimType: IEntityId
    {
        /// <summary>
        /// Gets or sets the type of claim.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        string Type { get; set; }
    }
}
