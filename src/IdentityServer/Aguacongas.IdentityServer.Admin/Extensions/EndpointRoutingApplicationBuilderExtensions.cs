
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
            string authicationScheme = "Bearer")
        {
            var entityTypeList = Utils.GetEntityTypeList();

            return builder.Map(basePath, child =>
            {
                configure(child);
                AuthenticateUserMiddleware(child, basePath, authicationScheme);
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
                        await response.CompleteAsync()
                            .ConfigureAwait(false);
                        return;
                    }
                }
                await next()
                    .ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Uses the identity server admin authentication.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="basePath">The base path.</param>
        /// <param name="authicationScheme">The authication scheme.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseIdentityServerAdminAuthentication(this IApplicationBuilder builder,
            string basePath, string authicationScheme)
        {
            return builder.Use((context, next) =>
            {
                return Authenticate(context, next, basePath, authicationScheme);
            });
        }

        private static void AuthenticateUserMiddleware(IApplicationBuilder child, string basePath, string authicationScheme)
        {
            child.UseRouting()
                .UseIdentityServerAdminAuthentication(basePath, authicationScheme)
                .UseAuthorization()
                .UseEndpoints(enpoints =>
                {
                    enpoints.MapAdminApiControllers();
                });
        }

        private static async Task Authenticate(HttpContext context, Func<Task> next, string basePath, string authicationScheme)
        {
            var resquest = context.Request;

            if ((resquest.PathBase.StartsWithSegments(basePath) || resquest.Path.StartsWithSegments(basePath)) && 
                !resquest.Method.Equals("option", StringComparison.OrdinalIgnoreCase) &&
                !context.User.Identity.IsAuthenticated)
            {
                var result = await context.AuthenticateAsync(authicationScheme)
                    .ConfigureAwait(false);

                if (result.Succeeded)
                {
                    context.User = result.Principal;
                }
                else
                {
                    var response = context.Response;
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await response.CompleteAsync()
                        .ConfigureAwait(false);
                    return;
                }
            }
            await next()
                .ConfigureAwait(false);
        }
    }
}
