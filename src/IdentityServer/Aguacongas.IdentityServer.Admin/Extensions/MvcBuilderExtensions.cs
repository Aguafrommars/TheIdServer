using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin;
using Aguacongas.IdentityServer.Admin.Filters;
using Aguacongas.IdentityServer.Admin.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IMvcBuilder"/> extensions
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds the identity server admin.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IMvcBuilder AddIdentityServerAdmin(this IMvcBuilder builder)
        {
            var assembly = typeof(MvcBuilderExtensions).Assembly;
            builder.Services.AddTransient<IPersistedGrantService, PersistedGrantService>()
                .AddTransient<SendGridEmailSender>()
                .AddSingleton<HubConnectionFactory>()                
                .AddTransient<IProviderClient>(p =>
                {
                    var hubConnection = p.GetRequiredService<HubConnectionFactory>().GetConnection();
                    if (hubConnection == null)
                    {
                        return null;
                    }

                    return new ProviderClient(hubConnection);
                })
                .AddSwaggerDocument(config =>
                {
                    config.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "IdentityServer4 admin API";
                        document.Info.Description = "A web API to administrate your IdentityServer4";
                        document.Info.Contact = new NSwag.OpenApiContact
                        {
                            Name = "Olivier Lefebvre",
                            Email = "olivier.lefebvre@live.com",
                            Url = "https://github.com/aguacongas"
                        };
                        document.Info.License = new NSwag.OpenApiLicense
                        {
                            Name = "Apache License 2.0",
                            Url = "https://github.com/aguacongas/TheIdServer/blob/master/LICENSE"
                        };
                    };
                    config.SerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                    var provider = builder.Services.BuildServiceProvider();
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    var authority = configuration.GetValue<string>("ApiAuthentication:Authority").Trim('/');
                    var apiName = configuration.GetValue<string>("ApiAuthentication:ApiName");
                    config.AddSecurity("oauth", new NSwag.OpenApiSecurityScheme
                    {
                        Flow = NSwag.OpenApiOAuth2Flow.Application,
                        Flows = new NSwag.OpenApiOAuthFlows(),
                        Scopes = new Dictionary<string, string>
                        {
                            [apiName] = "Api full access"
                        },
                        Description = "IdentityServer4",
                        Name = "IdentityServer4",
                        Scheme = "Bearer",
                        Type = NSwag.OpenApiSecuritySchemeType.OAuth2,
                        AuthorizationUrl = $"{authority}/connect/authorize",
                        TokenUrl = $"{authority}/connect/token",
                        OpenIdConnectUrl = authority
                    });
                    config.AddOperationFilter(context =>
                    {
                        context.OperationDescription.Operation.Security = new List<NSwag.OpenApiSecurityRequirement>
                        {
                            new NSwag.OpenApiSecurityRequirement
                            {
                                ["oauth"] = new string[] { apiName }
                            }
                        };
                        return true;
                    });
                });
            return builder.AddApplicationPart(assembly)
                .ConfigureApplicationPartManager(apm =>
                    apm.FeatureProviders.Add(new GenericApiControllerFeatureProvider()));
        }

        /// <summary>
        /// Adds the identity server admin filters.
        /// </summary>
        /// <param name="options">The options.</param>
        public static void AddIdentityServerAdminFilters(this MvcOptions options)
        {
            var filters = options.Filters;
            filters.Add<SelectFilter>();
            filters.Add<ExceptionFilter>();
            filters.Add<ActionsFilter>();
        }
    }
}
