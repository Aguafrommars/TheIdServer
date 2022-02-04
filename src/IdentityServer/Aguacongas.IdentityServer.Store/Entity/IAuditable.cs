// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Auditable interface
    /// </summary>
    public interface IAuditable : IEntityId
    {
        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the modified at.
        /// </summary>
        /// <value>
        /// The modified at.
        /// </value>
        DateTime? ModifiedAt { get; set; }
    }
}
