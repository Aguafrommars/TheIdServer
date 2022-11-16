﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddTheIdServerApi(this WebApplicationBuilder webApplicationBuilder)
        {
            var configuration = webApplicationBuilder.Configuration;
            var migrationsAssembly = "Aguacongas.TheIdServer.Migrations.SqlServer";
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var services = webApplicationBuilder.Services;
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString))
                .AddTheIdServerAdminEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddConfigurationEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            var signalRBuilder = services.AddSignalR(options => configuration.GetSection("SignalR:HubOptions").Bind(options));
            if (configuration.GetValue<bool>("SignalR:UseMessagePack"))
            {
                signalRBuilder.AddMessagePackProtocol();
            }


            services.Configure<SendGridOptions>(configuration)
                .AddControllersWithViews(options =>
                {
                    options.AddIdentityServerAdminFilters();
                })
                .AddNewtonsoftJson(options =>
                {
                    var settings = options.SerializerSettings;
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddIdentityServerAdmin<ApplicationUser, SchemeDefinition>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(SharedConstants.WRITERPOLICY, policy =>
                {
                    policy.RequireAssertion(context =>
                       context.User.IsInRole(SharedConstants.WRITERPOLICY));
                });
                options.AddPolicy(SharedConstants.READERPOLICY, policy =>
                {
                    policy.RequireAssertion(context =>
                       context.User.IsInRole(SharedConstants.READERPOLICY));
                });
            })
                .AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = "https://localhost:7443";
                    options.RequireHttpsMetadata = false;
                    options.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Both;
                    options.ApiName = "theidserveradminapi";
                    options.ApiSecret = "5b556f7c-b3bc-4b5b-85ab-45eed0cb962d";
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                    options.LegacyAudienceValidation = true;
                })
                .AddDynamic<SchemeDefinition>()
                .AddGoogle()
                .AddFacebook()
                .AddOpenIdConnect()
                .AddTwitter()
                .AddMicrosoftAccount()
                .AddOAuth("OAuth", options =>
                {
                });


            services.AddDatabaseDeveloperPageExceptionFilter()
                .AddResponseCompression(opts =>
                {
                    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "application/octet-stream" });
                });

            return webApplicationBuilder;
        }
    }
}
