
using Aguacongas.IdentityServer.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Contains extension methods for using IdenetityServer admin api controllers with <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class EndpointRoutingApplicationBuilderExtensions
    {
        /// <summary>
        /// Uses the identity server admin API.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="basePath">The base path.</param>
        /// <param name="configure">The configure.</param>
        /// <param name="authicationScheme">(Optional) authentication scheme to use</param>
        /// <returns></returns>
        public static IApplicationBuilder UseIdentityServerAdminApi(this IApplicationBuilder builder,
            string basePath, 
            Action<IApplicationBuilder> configure,
            string authicationScheme = null)
        {
            var entityTypeList = Utils.GetEntityTypeList();

            Func<HttpContext, Task<AuthenticateResult>> authenticate;
            if (authicationScheme != null)
            {
                authenticate = context => context.AuthenticateAsync(authicationScheme);
            }
            else
            {
                authenticate = context => context.AuthenticateAsync();
            }

            return builder.Map(basePath, child =>
                {
                    configure(child);
                    child.UseRouting()
                        .Use(async (context, next) =>
                        {
                            if (!context.Request.Method.Equals("option", StringComparison.OrdinalIgnoreCase) && 
                                !context.User.Identity.IsAuthenticated)
                            {
                                var result = await authenticate(context);
                                if (result.Succeeded)
                                {
                                    context.User = result.Principal;
                                }
                                else
                                {
                                    var response = context.Response;
                                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                                    await response.CompleteAsync();
                                    return;
                                }
                            }
                            await next();
                        })
                        .UseAuthorization()
                        .UseEndpoints(enpoints =>
                        {
                            enpoints.MapAdminApiControllers();
                        });
                })
                .Use(async (context, next) =>
                {
                    // avoid accessing the api outside the path.
                    var path = context.Request.Path;
                    if (path.HasValue)
                    {
                        var segments = path.Value.Split('/');
                        if (path.Equals(basePath, StringComparison.OrdinalIgnoreCase) || 
                            (!path.StartsWithSegments(basePath) && 
                                segments.Any(s => entityTypeList.Any(t => t.Name.Equals(s, StringComparison.OrdinalIgnoreCase)))))
                        {
                            var response = context.Response;
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            await response.CompleteAsync();
                            return;
                        }
                    }
                    await next();
                });
        }
    }
}
