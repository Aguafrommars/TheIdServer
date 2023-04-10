// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWsFederationSample(this IApplicationBuilder app)
        => app.UseDeveloperExceptionPage()
            .UseHttpsRedirection()
            .UseStaticFiles()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(enpoints => enpoints.MapDefaultControllerRoute());
    }
}
