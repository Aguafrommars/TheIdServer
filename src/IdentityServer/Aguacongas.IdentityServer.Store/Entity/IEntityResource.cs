// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Entity resource interface
    /// </summary>
    public interface IEntityResource : IEntityId
    {
        /// <summary>
        /// Gets or sets the kind of the resource.
        /// </summary>
        /// <value>
        /// The kind of the resource.
        /// </value>
        EntityResourceKind ResourceKind { get; set; }

        /// <summary>
        /// Gets or sets the culture identifier.
        /// </summary>
        /// <value>
        /// The culture identifier.
        /// </value>
        string CultureId { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        string Value { get; set; }
    }
}
