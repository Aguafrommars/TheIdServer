// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Api;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Services.KeyManagement;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;

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

            services.AddIdentityProviderStore()
                .AddConfigurationStores()
                .AddOperationalStores();

            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    configuration.Bind(nameof(IdentityOptions), options);
                })
            .AddTheIdServerStores()
            .AddDefaultTokenProviders();

            var signalRBuilder = services.AddSignalR(options => configuration.GetSection("SignalR:HubOptions").Bind(options));
            if (configuration.GetValue<bool>("SignalR:UseMessagePack"))
            {
                signalRBuilder.AddMessagePackProtocol();
            }

            services.Configure<SendGridOptions>(configuration)
                .AddLocalization()
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
                .AddIdentityServerAdmin<ApplicationUser, SchemeDefinition>()
                .AddTheIdServerEntityFrameworkStore();

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
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:7443";
                    options.Audience = "theidserveradminapi";
                });


            services.AddDatabaseDeveloperPageExceptionFilter()
                .AddResponseCompression(opts =>
                {
                    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "application/octet-stream" });
                });

            services.AddTransient<ISchemeChangeSubscriber, SchemeChangeSubscriber>();

            services.Configure<IdentityServerOptions>(options => options.IssuerUri = "https://localhost:5443")
                .AddTransient(p => p.GetRequiredService<IOptions<IdentityServerOptions>>().Value)
                .AddTransient<IClientConfigurationValidator, EmptyClientConfigurationValidator>()
                .AddTransient<IEventService, EmptyEventService>();

            services.RemoveAll<ICreatePersonalAccessToken>();
            services.AddTransient<ICreatePersonalAccessToken, CreatePersonalAccessTokenServvice>();

            services.AddScoped<IAuthenticationSchemeOptionsSerializer, AuthenticationSchemeOptionsSerializer>();

            services.AddTransient(p =>
            {
                var handler = new HttpClientHandler();
                if (configuration.GetValue<bool>("DisableStrictSsl"))
                {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, policy) => true;
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
                }
                return handler;
            });

            return webApplicationBuilder;
        }
    }
}
