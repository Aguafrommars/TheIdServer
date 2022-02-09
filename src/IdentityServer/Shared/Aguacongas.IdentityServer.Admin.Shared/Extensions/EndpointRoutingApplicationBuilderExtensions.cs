// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
#if DUENDE
using Duende.IdentityServer.Extensions;
#else
using IdentityServer4.Extensions;
#endif
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
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

            ByBassAuthentication(context, request, path);

            if (!await GetRegistrationTokenAsync(context, request, path).ConfigureAwait(false))
            {
                return;
            }

            var logger = context.RequestServices.GetRequiredService<ILogger<HttpContext>>();            
            if (context.User.Identity.IsAuthenticated)
            {
                using var authscope = logger.BeginScope(new Dictionary<string, object> { ["User"] = context.User.GetDisplayName() });
                await next().ConfigureAwait(false);
                return;
            }

            var result = await context.AuthenticateAsync(authicationScheme)
                    .ConfigureAwait(false);

            context.User = result.Principal;
            using var scope = logger.BeginScope(new Dictionary<string, object> { ["User"] = context.User.GetDisplayName() });

            if (!result.Succeeded && 
                path.StartsWithSegments("/register", StringComparison.OrdinalIgnoreCase) &&
                request.Method != HttpMethods.Post)
            {
                await SetForbiddenResponse(context).ConfigureAwait(false);
                return;
            }


            await next().ConfigureAwait(false);
        }

        private static async Task SetForbiddenResponse(HttpContext context)
        {
            var response = context.Response;
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            await response.CompleteAsync()
                .ConfigureAwait(false);
        }

        private static void ByBassAuthentication(HttpContext context, HttpRequest request, PathString path)
        {
            if ((path.StartsWithSegments("/welcomefragment", StringComparison.OrdinalIgnoreCase) ||
                            path.StartsWithSegments($"/{nameof(Culture)}", StringComparison.OrdinalIgnoreCase) ||
                            path.StartsWithSegments($"/{nameof(LocalizedResource)}", StringComparison.OrdinalIgnoreCase)) &&
                            !context.User.IsInRole(SharedConstants.READERPOLICY) &&
                            !context.User.HasClaim(c => c.Type == JwtClaimTypes.Role && c.Value == SharedConstants.ADMINSCOPE) &&
                            request.Method == HttpMethods.Get)
            {
                // by-pass security for localized resource read
                context.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                            new Claim(JwtClaimTypes.Name, "AnonymousReader"),
                            new Claim(JwtClaimTypes.Role, SharedConstants.READERPOLICY),
                            new Claim(JwtClaimTypes.Scope, SharedConstants.ADMINSCOPE)
                }, "by-pass", JwtClaimTypes.Name, JwtClaimTypes.Role));
            }
        }

        private static async Task<bool> GetRegistrationTokenAsync(HttpContext context, HttpRequest request, PathString path)
        {
            if (path.StartsWithSegments("/register", StringComparison.OrdinalIgnoreCase) &&
                request.Method != HttpMethods.Post)
            {
                if (!request.Headers.TryGetValue(HeaderNames.Authorization, out StringValues authorizationHeaderValue))
                {
                    await SetForbiddenResponse(context).ConfigureAwait(false);
                    return false;
                }

                // get token for registration end point
                if (!Guid.TryParse(authorizationHeaderValue.First().Split(' ')[1], out Guid token))
                {
                    // The token is not au GUID
                    await SetForbiddenResponse(context).ConfigureAwait(false);
                    return false;
                }

                var store = context.RequestServices.GetRequiredService<IAdminStore<Client>>();
                var clientResponse = await store.GetAsync(new PageRequest
                {
                    Filter = $"{nameof(Client.RegistrationToken)} eq {token}",
                    Select = nameof(Client.Id),
                    Take = 1
                }).ConfigureAwait(false);

                var client = clientResponse.Items.FirstOrDefault();
                if (client == null || path.Value.EndsWith(client.Id))
                {
                    context.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                                new Claim(JwtClaimTypes.Name, client?.Id ?? "not found"),
                                new Claim(JwtClaimTypes.Role, SharedConstants.REGISTRATIONPOLICY)
                    }, "registration", JwtClaimTypes.Name, "role"));
                    return true;
                }
                await SetForbiddenResponse(context).ConfigureAwait(false);
                return false;
            }
            return true;
        }
    }
}
