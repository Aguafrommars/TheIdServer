// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseWebClient(this IApplicationBuilder app)
    => app.UseDeveloperExceptionPage()
            .UseHttpsRedirection()
            .UseStaticFiles()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute()
                    .RequireAuthorization();
            });
}
