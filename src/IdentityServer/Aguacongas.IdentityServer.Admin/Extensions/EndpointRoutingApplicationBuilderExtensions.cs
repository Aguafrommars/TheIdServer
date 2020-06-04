
using Aguacongas.IdentityServer.Admin;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
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
        /// <param name="notAllowedApiRewritePath">(Optional) the rewritten path when an api route match outside the base path</param>
        /// <returns></returns>
        public static IApplicationBuilder UseIdentityServerAdminApi(this IApplicationBuilder builder,
            string basePath,
            Action<IApplicationBuilder> configure,
            string authicationScheme = "Bearer",
            string notAllowedApiRewritePath = "not-allowed")
        {
            var entityTypeList = Utils.GetEntityTypeList();

            return builder.Map(basePath, child =>
            {
                configure(child);
                AuthenticateUserMiddleware(child, basePath, authicationScheme);
            })
            .Use((context, next) =>
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
                        context.Request.Path = new PathString($"/{notAllowedApiRewritePath}");
                    }
                }
                return next();
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
            child
                .UseRouting()
                .UseIdentityServerAdminAuthentication(basePath, authicationScheme)
                .UseAuthorization()
                .UseEndpoints(enpoints =>
                {
                    enpoints.MapAdminApiControllers();
                });
        }

        private static async Task Authenticate(HttpContext context, Func<Task> next, string basePath, string authicationScheme)
        {
            var request = context.Request;
            var path = request.Path;

            if (request.Method.Equals("option", StringComparison.OrdinalIgnoreCase) ||
                !request.PathBase.StartsWithSegments(basePath) && !path.StartsWithSegments(basePath))
            {
                await next().ConfigureAwait(false);
                return;
            }

            if (path.StartsWithSegments($"/{nameof(LocalizedResource)}", StringComparison.OrdinalIgnoreCase) && 
                request.Method == HttpMethods.Get &&
                !context.User.IsInRole(SharedConstants.READER))
            {
                // by-pass security for localized resource read
                context.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                            new Claim(JwtClaimTypes.Name, "AnonymousReader"),
                            new Claim("role", SharedConstants.READER)
                }, "by-pass", JwtClaimTypes.Name, "role"));
            }


            if (context.User.Identity.IsAuthenticated)
            {
                await next().ConfigureAwait(false);
                return;
            }

            var result = await context.AuthenticateAsync(authicationScheme)
                    .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                await response.CompleteAsync()
                    .ConfigureAwait(false);
                return;
            }

            context.User = result.Principal;

            await next().ConfigureAwait(false);
        }
    }
}
