using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApiHost.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddApi(this WebApplicationBuilder webApplicationBuilder)
    {
        var services = webApplicationBuilder.Services;

        services.AddControllers();
        services.AddCors();

        // this API will accept any access token from the authority
        services.AddAuthentication("token")
            .AddJwtBearer("token", options =>
            {
                options.Authority = "https://localhost:5443";
                options.TokenValidationParameters.ValidateAudience = false;
                options.MapInboundClaims = false;

                options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
            });

        // layers DPoP onto the "token" scheme above
        services.ConfigureDPoPTokensForScheme("token");

        return webApplicationBuilder;
    }
}
