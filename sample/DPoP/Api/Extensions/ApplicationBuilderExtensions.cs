using Microsoft.AspNetCore.Builder;

namespace ApiHost.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApi(this IApplicationBuilder app)
    => app.UseRouting()
        .UseAuthentication()
        .UseAuthorization()
        .UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireAuthorization();
        });
}
