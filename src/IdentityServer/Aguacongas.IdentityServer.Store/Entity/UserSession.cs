// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// User session
    /// </summary>
    public class UserSession : IEntityId, IUserSubEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// The cookie handler scheme
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// The session ID
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// The display name for the user
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The creation time
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// The renewal time
        /// </summary>
        public DateTime Renewed { get; set; }

        /// <summary>
        /// The expiration time
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// The serialized ticket
        /// </summary>
        public string Ticket { get; set; }

    }
}
