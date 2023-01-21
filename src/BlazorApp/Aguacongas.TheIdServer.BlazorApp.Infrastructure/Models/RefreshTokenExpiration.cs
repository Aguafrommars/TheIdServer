// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Refresh token expiration
    /// </summary>
    public enum RefreshTokenExpiration
    {
        /// <summary>
        /// Sliding expiration
        /// </summary>
        Sliding = 0,
        /// <summary>
        /// Absolute expiration
        /// </summary>
        Absolute = 1
    }
}
