// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Newtonsoft.Json;

namespace Aguacongas.IdentityServer.Admin.Models
{
    /// <summary>
    /// Defines an entity metadate
    /// </summary>
    public class EntityMetadata
    {
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        [JsonProperty("_metadata")]
        public Metadata Metadata { get; set; }
    }

    /// <summary>
    /// Defines a metadata
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Gets or sets the entity type's name.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string TypeName { get; set; }
    }
}
