﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.Admin.Options
{
    /// <summary>
    /// Defines dynamic client registration options.
    /// </summary>
    public class DynamicClientRegistrationOptions
    {
        /// <summary>
        /// Gets or sets the allowed contacts.
        /// </summary>
        /// <value>
        /// The allowed contacts.
        /// </value>
        public IEnumerable<AllowedContact> AllowedContacts { get; set; }
    }

    /// <summary>
    /// Defines dynamic client registration allowed contact.
    /// </summary>
    public class AllowedContact
    {
        /// <summary>
        /// Gets or sets the contact.
        /// </summary>
        /// <value>
        /// The contact.
        /// </value>
        public string Contact { get; set; }

        /// <summary>
        /// Gets or sets the allowed hosts.
        /// </summary>
        /// <value>
        /// The allowed hosts.
        /// </value>
        public IEnumerable<string> AllowedHosts { get; set; }
    }
}
