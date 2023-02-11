// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Defines a culture
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class Culture : IAuditable, ICloneable<Culture>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the resources collection.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public virtual ICollection<LocalizedResource> Resources { get; set; }

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

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Culture Clone()
        {
            return MemberwiseClone() as Culture;
        }
    }
}
