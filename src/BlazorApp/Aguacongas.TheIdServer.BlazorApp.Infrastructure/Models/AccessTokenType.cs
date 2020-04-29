namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Access token type
    /// </summary>
    public enum AccessTokenType
    {
        /// <summary>
        /// Self-contained Json Web Token
        /// </summary>
        Jwt = 0,
        /// <summary>
        /// Reference token
        /// </summary>
        Reference = 1
    }
}
