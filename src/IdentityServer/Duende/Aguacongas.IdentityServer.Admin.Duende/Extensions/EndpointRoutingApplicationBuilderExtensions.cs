// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
using Duende.IdentityServer.Extensions;
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
using Aguacongas.IdentityServer.Admin.Configuration;

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
        /// <returns></returns>
        public static IApplicationBuilder UseIdentityServerAdminApi(this IApplicationBuilder builder,
            string basePath,
            Action<IApplicationBuilder> configure)
        {
            ApiBasePath.Value = basePath;

            configure(builder);
            return builder;
        }

        /// <summary>
        /// Uses the identity server admin authentication.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="basePath">The base path.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseIdentityServerAdminAuthentication(this IApplicationBuilder builder, string basePath = null)
        {
            return builder.Use((context, next) =>
            {
                return Authenticate(context, next, basePath ?? ApiBasePath.Value);
            });
        }

        private static async Task Authenticate(HttpContext context, Func<Task> next, string basePath)
        {
            var request = context.Request;
            var path = request.Path;

            if (request.Method.Equals("option", StringComparison.OrdinalIgnoreCase) || 
                !path.HasValue ||
                !path.StartsWithSegments(basePath))
            {
                await next().ConfigureAwait(false);
                return;
            }

            if (!context.User.IsAuthenticated())
            {
                await AuthenticateUsingAuthorizationHeader(context, request).ConfigureAwait(false);
            }

            ByBassAuthentication(context, request, path, basePath);

            if (!await GetRegistrationTokenAsync(context, request, path, basePath).ConfigureAwait(false))
            {
                return;
            }

            var logger = context.RequestServices.GetRequiredService<ILogger<HttpContext>>();            
            
            using var scope = logger.BeginScope(new Dictionary<string, object> { ["User"] = context.User.GetDisplayName() });
            await next().ConfigureAwait(false);

            var response = context.Response;
            if (context.Response.StatusCode == (int)HttpStatusCode.Redirect) 
            {
                response.StatusCode = context.User.IsAuthenticated() ? (int)HttpStatusCode.Unauthorized : (int)HttpStatusCode.Forbidden;
                await response.CompleteAsync().ConfigureAwait(false);
            }
        }

        private static async Task AuthenticateUsingAuthorizationHeader(HttpContext context, HttpRequest request)
        {
            var authorizationHeader = request.Headers.Authorization;
            var authenticationScheme = authorizationHeader.FirstOrDefault();

            if (authenticationScheme is null)
            {
                return;
            }

            var result = await context.AuthenticateAsync(authenticationScheme.Split(' ')[0]).ConfigureAwait(false);

            if (result.Succeeded)
            {
                context.User = result.Principal;
            }
        }

        private static async Task SetForbiddenResponse(HttpContext context)
        {
            var response = context.Response;
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            await response.CompleteAsync()
                .ConfigureAwait(false);
        }

        private static void ByBassAuthentication(HttpContext context, HttpRequest request, PathString path, string basePath)
        {
            if ((path.StartsWithSegments($"{basePath}/welcomefragment", StringComparison.OrdinalIgnoreCase) ||
                            path.StartsWithSegments($"{basePath}/{nameof(Culture)}", StringComparison.OrdinalIgnoreCase) ||
                            path.StartsWithSegments($"{basePath}/{nameof(LocalizedResource)}", StringComparison.OrdinalIgnoreCase)) &&
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

        private static async Task<bool> GetRegistrationTokenAsync(HttpContext context, HttpRequest request, PathString path, string basePath)
        {
            if (path.StartsWithSegments($"{basePath}/register", StringComparison.OrdinalIgnoreCase) &&
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
