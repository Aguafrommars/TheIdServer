using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Aguacongas.TheIdServer.MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var clientId = "mvc";
            var clientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
            var authority = "https://localhost:5443";

            services.AddTransient<HttpClient>()
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies", options =>
                {
                    var pendingRefreshTokenRequests = new ConcurrentDictionary<string, bool>();

                    var events = options.Events;                    
                    events.OnValidatePrincipal = async context =>
                    {
                        var tokens = context.Properties.GetTokens();
                        var services = context.HttpContext.RequestServices;
                        var logger = services.GetRequiredService<ILogger<Startup>>();
                        if (tokens == null || !tokens.Any())
                        {
                            logger.LogDebug("No tokens found.");
                            return;
                        }

                        var refreshToken = tokens.FirstOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);
                        if (refreshToken == null)
                        {
                            logger.LogWarning("Refresh token not found.");
                            return;
                        }

                        var expiresAt = tokens.FirstOrDefault(t => t.Name == "expires_at");
                        if (refreshToken == null)
                        {
                            logger.LogWarning("expires_at not found.");
                            return;
                        }

                        var expires = DateTimeOffset.Parse(expiresAt.Value, CultureInfo.InvariantCulture);
                        logger.LogInformation("Token expire at {ExpireAtt}", expires);
                        expires = expires.AddSeconds(-15);
                        if (expires > DateTimeOffset.UtcNow)
                        {
                            logger.LogDebug("No need to refresh");
                            return;
                        }

                        if (!pendingRefreshTokenRequests.TryAdd(refreshToken.Value, true))
                        {
                            logger.LogInformation("Pending refresh the token {RefreshToken}", refreshToken.Value);
                            return;
                        }

                        try
                        {
                            var httpClient = services.GetRequiredService<HttpClient>();
                            using var request = new RefreshTokenRequest
                            {
                                Address = $"{authority}/connect/token",
                                ClientId = clientId,
                                ClientSecret = clientSecret,
                                RefreshToken = refreshToken.Value
                            };
                            var response = await httpClient.RequestRefreshTokenAsync(request);

                            if (response.IsError)
                            {
                                logger.LogWarning("Error refreshing token: {error}", response.Error);
                                context.RejectPrincipal();
                                return;
                            }

                            context.Properties.UpdateTokenValue("access_token", response.AccessToken);
                            context.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);

                            expires = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn);
                            context.Properties.UpdateTokenValue("expires_at", expires.ToString("o", CultureInfo.InvariantCulture));

                            await context.HttpContext.SignInAsync(context.Principal, context.Properties);
                            logger.LogInformation("Automatic refresh token succeed. Next expire date {ExpireAt}", expires);
                        }
                        catch(Exception e)
                        {
                            logger.LogError(e.Message, e);
                        }
                        finally
                        {
                            pendingRefreshTokenRequests.TryRemove(refreshToken.Value, out _);
                        }
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = false;

                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                    options.ResponseType = "code id_token";
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("api1");
                    options.Scope.Add("offline_access");
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                });
           
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection()
                .UseAuthentication()
                .UseStaticFiles()
                .UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
