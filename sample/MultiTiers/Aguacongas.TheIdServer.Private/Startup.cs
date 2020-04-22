// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = Configuration.GetConnectionString("DefaultConnection");            
            
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString))
                .AddIdentityServer4AdminEntityFrameworkStores<ApplicationUser, ApplicationDbContext>()
                .AddConfigurationEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddIdentityProviderStore();

            services.AddIdentity<ApplicationUser, IdentityRole>(
                    options => options.SignIn.RequireConfirmedAccount = Configuration.GetValue<bool>("SignInOptions:RequireConfirmedAccount"))
                .AddEntityFrameworkStores<ApplicationDbContext>()
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

            var authBuilder = services
                .AddAuthorization(options =>
                    options.AddIdentityServerPolicies())
                .AddAuthentication()
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    Configuration.GetSection("ApiAuthentication").Bind(options);
                });


            authBuilder.AddDynamic<SchemeDefinition>()
                .AddEntityFrameworkStore<ConfigurationDbContext>()
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

            services.AddControllersWithViews(options =>
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
                .UseStaticFiles()
                .UseIdentityServer()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapRazorPages();
                }); 
        }
    }
}