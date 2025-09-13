// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Identity localized resource
    /// </summary>
    /// <seealso cref="LocalizedResourceBase" />
    /// <seealso cref="IIdentitySubEntity" />
    public class IdentityLocalizedResource : LocalizedResourceBase, IIdentitySubEntity, IEntityResource
    {
        /// <summary>
        /// Gets or sets the identity resource identifier.
        /// </summary>
        /// <value>
        /// The API identifier.
        /// </value>
        public string IdentityId { get; set; }


        /// <summary>
        /// Gets or sets the kind of the resource.
        /// </summary>
        /// <value>
        /// The kind of the resource.
        /// </value>
        public EntityResourceKind ResourceKind { get; set; }


        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        public virtual IdentityResource Identity { get; set; }
    }
}
