using NSwag.AspNetCore;

namespace Aguacongas.TheIdServer.Duende;

/// <summary>
/// Represents settings for configuring the Swagger UI in the application.
/// </summary>
public class SwaggerUiSettings
{
    /// <summary>
    /// Gets or sets the Swagger UI OAuth2 client settings.
    /// </summary>
    public OAuth2ClientSettings? OAuth2Client { get; set; }

    /// <summary>
    /// Gets or sets the internal swagger UI route (must start with '/').
    /// </summary>
    public string Path { get; set; } = "/swagger";

}
