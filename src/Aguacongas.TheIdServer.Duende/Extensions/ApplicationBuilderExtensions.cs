// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Aguacongas.TheIdServer.Options.OpenTelemetry;
using Duende.IdentityServer.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Serilog;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTheIdServer(this IApplicationBuilder app, IWebHostEnvironment environment, IConfiguration configuration)
        {
            var isProxy = configuration.GetValue<bool>("Proxy");
            var dbType = configuration.GetValue<DbTypes>("DbType");

            var disableHttps = configuration.GetValue<bool>("DisableHttps");

            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("ApplicationBuilderExtensions");
            var certificateHeader = configuration.GetValue<string>($"{nameof(IdentityServerOptions)}:{nameof(IdentityServerOptions.MutualTls)}:PEMHeader");

            if (!string.IsNullOrEmpty(certificateHeader))
            {
                logger.LogDebug("Get client certificate from header {ClientCertificateHeader}", certificateHeader);
                app.UseClientCerificate(certificateHeader);
            }

            app.UseForwardedHeaders();
            AddForceHttpsSchemeMiddleware(app, configuration);

            if (!isProxy)
            {
                ConfigureInitialData(app, configuration, dbType);
            }

            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                if (!disableHttps)
                {
                    app.UseHsts();
                }
            }

            var scope = app.ApplicationServices.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var supportedCulture = scopedProvider.GetRequiredService<ISupportCultures>().CulturesNames.ToArray();

            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCulture.Select(c => new CultureInfo(c)).ToList();
                options.SupportedUICultures = options.SupportedCultures;
                options.FallBackToParentCultures = true;
                options.AddInitialRequestCultureProvider(new SetCookieFromQueryStringRequestCultureProvider());
            })
                .UseSerilogRequestLogging();

            if (!disableHttps)
            {
                app.UseHttpsRedirection();
            }

            app.UseBlazorFrameworkFiles()
               .UseStaticFiles()
               .UseIdentityServerAdminApi("/api", child =>
               {
                   ConfigureAdminApiHandler(configuration, child);
               });

            app.UseIdentityServer()
                .UseRouting()
                .UseAuthentication();

            if (!isProxy)
            {
                app.UseIdentityServerAdminAuthentication("/providerhub");
            }

            app.UseIdentityServerAdminAuthentication()
                .UseAuthorization()
                .Use((context, next) =>
                {
                    var service = context.RequestServices;
                    var settings = service.GetRequiredService<Settings>();
                    var request = context.Request;
                    settings.WelcomeContenUrl = $"{request.Scheme}://{request.Host}/api/welcomefragment";
                    var remotePathOptions = service.GetRequiredService<IOptions<RemoteAuthenticationApplicationPathsOptions>>().Value;
                    remotePathOptions.RemoteProfilePath = $"{request.Scheme}://{request.Host}/identity/account/manage";
                    remotePathOptions.RemoteRegisterPath = $"{request.Scheme}://{request.Host}/identity/account/register";
                    return next();
                })
                .UsePrometheus(configuration)
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapAdminApiControllers();
                    endpoints.MapHealthChecks("healthz", new HealthCheckOptions
                    {
                        ResponseWriter = WriteHealtResponse
                    });

                    endpoints.MapRazorPages();
                    endpoints.MapDefaultControllerRoute();
                    if (!isProxy)
                    {
                        endpoints.MapHub<ProviderHub>("/providerhub");
                    }
                    endpoints.MapFallbackToPage("/_Host");
                });
                

            return app.LoadDynamicAuthenticationConfiguration<SchemeDefinition>();
        }

        private static void ConfigureAdminApiHandler(IConfiguration configuration, IApplicationBuilder child)
        {
            if (configuration.GetValue<bool>("EnableOpenApiDoc"))
            {
                child.UseOpenApi()
                    .UseSwaggerUi3(options =>
                    {
                        var settings = configuration.GetSection("SwaggerUiSettings").Get<NSwag.AspNetCore.SwaggerUiSettings>();
                        options.OAuth2Client = settings?.OAuth2Client;
                    });
            }
            var allowedOrigin = configuration.GetSection("CorsAllowedOrigin").Get<IEnumerable<string>>();
            if (allowedOrigin != null)
            {
                child.UseCors(configure =>
                {
                    configure.SetIsOriginAllowed(origin => allowedOrigin.Any(o => o == origin))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            };
        }

        private static IApplicationBuilder UseClientCerificate(this IApplicationBuilder app, string certificateHeader)
        {
            return app.Use(async (context, next) =>
            {
                var requestLoggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var requestLogger = requestLoggerFactory.CreateLogger("GetClientCertificateMiddleware");

                var headers = context.Request.Headers;
                using var scope = requestLogger.BeginScope(new Dictionary<string, object>
                {
                    ["Headers"] = headers
                });

                if (headers.TryGetValue(certificateHeader, out StringValues values))
                {
                    requestLogger.LogInformation("Get certificate from header {ClientCertificateHeader}", certificateHeader);
                    try
                    {
                        context.Connection.ClientCertificate = X509Certificate2.CreateFromPem(Uri.UnescapeDataString(values[0]));
                    }
                    catch (CryptographicException e)
                    {
                        requestLogger.LogWarning("Failed to get certificate fron header {ClientCertificateHeader}. Error: {Error}", certificateHeader, e.Message);
                    }
                }
                await next();
            });
        }
        private static IApplicationBuilder UsePrometheus(this IApplicationBuilder app, IConfiguration configuration)
        {
            var otlpOptions = configuration.GetSection(nameof(OpenTelemetryOptions)).Get<OpenTelemetryOptions>();
            var prometheusOtions = otlpOptions?.Metrics?.Prometheus;
            if (prometheusOtions is not null)
            {
                if (prometheusOtions.Protected)
                {
                    app.Use((context, next) =>
                    {
                        if (context.Request.Path.StartsWithSegments(prometheusOtions.ScrapeEndpointPath) &&
                            context.User?.IsInRole(SharedConstants.READERPOLICY) != true)
                        {
                            var response = context.Response;
                            response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return response.CompleteAsync();
                        }
                        return next();
                    });
                }

                app.UseOpenTelemetryPrometheusScrapingEndpoint();
            }

            return app;
        }

        private static void AddForceHttpsSchemeMiddleware(IApplicationBuilder app, IConfiguration configuration)
        {
            var forceHttpsScheme = configuration.GetValue<bool>("ForceHttpsScheme");

            if (forceHttpsScheme)
            {
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }
        }

        private static void ConfigureInitialData(IApplicationBuilder app, IConfiguration configuration, DbTypes dbType)
        {
            if (configuration.GetValue<bool>("Migrate") &&
                dbType != DbTypes.InMemory && dbType != DbTypes.RavenDb && dbType != DbTypes.MongoDb)
            {
                using var scope = app.ApplicationServices.CreateScope();
                var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configContext.Database.Migrate();

                var opContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
                opContext.Database.Migrate();

                var appcontext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                appcontext.Database.Migrate();
            }

            if (configuration.GetValue<bool>("Seed"))
            {
                using var scope = app.ApplicationServices.CreateScope();
                SeedData.SeedConfiguration(scope, configuration);
                SeedData.SeedUsers(scope, configuration);
            }

        }

        private static Task WriteHealtResponse(HttpContext context, HealthReport healthReport)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions { Indented = true };
            
            using var memoryStream = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(memoryStream, options);
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                var value = healthReportEntry.Value;

                jsonWriter.WriteString("status", value.Status.ToString());

                if (value.Description is not null)
                {
                    jsonWriter.WriteString("description",
                    value.Description);
                }

                if (value.Data.Any())
                {
                    var serializerOptions = new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    };

                    jsonWriter.WriteStartObject("data");

                    foreach (var item in value.Data)
                    {
                        jsonWriter.WritePropertyName(item.Key);

                        JsonSerializer.Serialize(jsonWriter, item.Value,
                            item.Value?.GetType() ?? typeof(object), serializerOptions);
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
            jsonWriter.Flush();

            return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
        }
    }
}
