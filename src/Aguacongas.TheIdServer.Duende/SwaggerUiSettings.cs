using NSwag.AspNetCore;

namespace Aguacongas.TheIdServer.Duende;

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
