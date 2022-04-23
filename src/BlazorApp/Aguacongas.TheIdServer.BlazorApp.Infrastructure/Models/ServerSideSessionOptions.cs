using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Configures the behavior for server-side sessions.
    /// </summary>
    public class ServerSideSessionOptions
    {
        /// <summary>
        /// The claim type used for the user's display name.
        /// </summary>
        public string UserDisplayNameClaimType { get; set; }

        /// <summary>
        /// If enabled, will perodically cleanup expired sessions.
        /// </summary>
        public bool RemoveExpiredSessions { get; set; }

        /// <summary>
        /// If enabled, when server-side sessions are removed due to expiration, will back-channel logout notifications be sent.
        /// This will, in effect, tie a user's session lifetime at a client to their session lifetime at IdentityServer.
        /// </summary>
        public bool ExpiredSessionsTriggerBackchannelLogout { get; set; }

        /// <summary>
        /// Frequency expired sessions will be removed.
        /// </summary>
        public TimeSpan RemoveExpiredSessionsFrequency { get; set; }

        /// <summary>
        /// Number of expired sessions records to be removed at a time.
        /// </summary>
        public int RemoveExpiredSessionsBatchSize { get; set; }
    }
}