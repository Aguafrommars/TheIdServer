// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class RelyingPartyClaimMapping : IAuditable
    {
        /// <summary>
        /// Gets or sets the realm.
        /// </summary>
        /// <value>
        /// The realm.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// The relying party identifier
        /// </summary>
        public string RelyingPartyId{ get; set; }

    /// <summary>
    /// Gets or sets the type of from claim.
    /// </summary>
    /// <value>
    /// The type of from claim.
    /// </value>
        [Required]
        public string FromClaimType { get; set; }

        /// <summary>
        /// Converts to claimtype.
        /// </summary>
        /// <value>
        /// The type of to claim.
        /// </value>
        [Required]
        public string ToClaimType { get; set; }

        /// <summary>
        /// Gets or sets the relying party.
        /// </summary>
        /// <value>
        /// The relying party.
        /// </value>
        public virtual RelyingParty RelyingParty { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the modified at.
        /// </summary>
        /// <value>
        /// The modified at.
        /// </value>
        public DateTime? ModifiedAt { get; set; }
    }
}
