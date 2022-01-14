// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Define a device code
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class DeviceCode : IAuditable
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
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [MaxLength(200)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the user code.
        /// </summary>
        /// <value>
        /// The user code.
        /// </value>
        [MaxLength(200)]
        public string UserCode { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        [MaxLength(200)]
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>
        /// The expiration.
        /// </value>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data { get; set; }

        /// <summary>
        /// Gets the description the user assigned to the device being authorized.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        [MaxLength(100)]
        public string SessionId { get; set; }

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
