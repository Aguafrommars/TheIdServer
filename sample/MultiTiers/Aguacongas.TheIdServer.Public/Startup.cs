// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Http.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Aguacongas.TheIdServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            void configureOptions(IdentityServerOptions options) 
                => Configuration.GetSection("PrivateServerAuthentication").Bind(options);

            services.AddTransient<HttpClientHandler>();

            services.AddIdentityProviderStore()
                .AddConfigurationHttpStores(configureOptions)
                .AddOperationalHttpStores()
                .AddIdentity<ApplicationUser, IdentityRole>(
                    options => options.SignIn.RequireConfirmedAccount = Configuration.GetValue<bool>("SignInOptions:RequireConfirmedAccount"))
                .AddTheIdServerStores(configureOptions)
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddDefaultSecretParsers()
                .AddDefaultSecretValidators()
                .AddSigningCredentials();

            var authBuilder = services.AddAuthorization(options =>
                    options.AddIdentityServerPolicies())
                .AddAuthentication()
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    Configuration.GetSection("ApiAuthentication").Bind(options);
                });


            authBuilder.AddDynamic<SchemeDefinition>()
                .AddTheIdServerHttpStore()
                .AddGoogle()
                .AddFacebook()
                .AddOpenIdConnect()
                .AddTwitter()
                .AddMicrosoftAccount()
                .AddOAuth("OAuth", options =>
                {
                    options.ClaimActions.MapAll();
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var contextOption = context.Options;
                            if (string.IsNullOrEmpty(contextOption.UserInformationEndpoint))
                            {
                                return;
                            }

                            var request = new HttpRequestMessage(HttpMethod.Get, contextOption.UserInformationEndpoint);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("DynamicAuthProviders-sample", "1.0.0"));

                            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                            var content = await response.Content.ReadAsStringAsync();
                            response.EnsureSuccessStatusCode();

                            using var doc = JsonDocument.Parse(content);

                            context.RunClaimActions(doc.RootElement);
                        }
                    };
                });

            services.Configure<SendGridOptions>(Configuration)
                .AddControllersWithViews(options =>
                    options.AddIdentityServerAdminFilters())
                .AddNewtonsoftJson(options =>
                {
                    var settings = options.SerializerSettings;
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddIdentityServerAdmin();
            services.AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("Identity", "/Account"));
        }

        [SuppressMessage("Usage", "ASP0001:Authorization middleware is incorrectly configured.", Justification = "<Pending>")]
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error")
                    .UseHsts();
            }

            app.UseSerilogRequestLogging()
                .UseHttpsRedirection()
                .UseIdentityServerAdminApi("/api", child =>
                {
                    if (Configuration.GetValue<bool>("EnableOpenApiDoc"))
                    {
                        child.UseOpenApi()
                            .UseSwaggerUi3(options =>
                            {
                                var settings = Configuration.GetSection("SwaggerUiSettings").Get<NSwag.AspNetCore.SwaggerUiSettings>();
                                options.OAuth2Client = settings.OAuth2Client;
                            });
                    }
                    var allowedOrigin = Configuration.GetSection("CorsAllowedOrigin").Get<IEnumerable<string>>();
                    if (allowedOrigin != null)
                    {
                        child.UseCors(configure =>
                        {
                            configure.SetIsOriginAllowed(origin => allowedOrigin.Any(o => o == origin))
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
                    }
                })
                .UseBlazorFrameworkFiles()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthentication()
                .UseIdentityServer()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapFallbackToFile("index.html");
                })
                .LoadDynamicAuthenticationConfiguration<SchemeDefinition>();
        }
    }
}