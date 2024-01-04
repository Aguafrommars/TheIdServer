// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin;
using Aguacongas.IdentityServer.Admin.Filters;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.Store;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

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
        public static DynamicAuthenticationBuilder AddIdentityServerAdmin<TUser, TSchemeDefinition>(this IMvcBuilder builder) 
            where TUser : IdentityUser, new()
            where TSchemeDefinition: SchemeDefinitionBase, new()
        {
            var services = builder.Services;
            var assembly = typeof(MvcBuilderExtensions).Assembly;
            services.AddHostedService<SchemeChangeHost>()
                .AddSingleton<HubConnectionFactory>()
                .AddSingleton<StringLocalizerFactory>()
                .AddSingleton<IStringLocalizerFactory>(p => p.GetRequiredService<StringLocalizerFactory>())
                .AddSingleton<ISupportCultures>(p => p.GetRequiredService<StringLocalizerFactory>())
                .AddTransient<IPersistedGrantService, PersistedGrantService>()
                .AddTransient<SendGridEmailSender>()
                .AddTransient<IProviderClient, ProviderClient>()                
                .AddTransient(p => new HubHttpMessageHandlerAccessor { Handler = p.GetRequiredService<HttpClientHandler>() })
                .AddTransient<ExternalClaimsTransformer<TUser>>()
                .AddTransient<IProxyClaimsProvider, ProxyClaimsProvider<TUser>>()
                .AddTransient<IRetrieveOneTimeToken, OneTimeTokenService>()
                .AddTransient<IImportService, ImportService>()
                .AddTransient<ICertificateVerifierService, CertificateVerifierService>()
                .AddTransient<ICreatePersonalAccessToken, CreatePersonalAccessTokenService>()
                .AddTransient(p => new KeyManagerWrapper<IAuthenticatedEncryptorDescriptor>(new[] { new Tuple<IKeyManager, string, IEnumerable<IKey>>(p.GetRequiredService<IKeyManager>(), "dataprotection", p.GetRequiredService<IKeyManager>().GetAllKeys()) }, 
                    p.GetRequiredService<IDefaultKeyResolver>(), 
                    p.GetRequiredService<IProviderClient>()))
                .AddTransient(p => new KeyManagerWrapper<RsaEncryptorDescriptor>(p.GetService<IEnumerable<Aguacongas.IdentityServer.KeysRotation.ICacheableKeyRingProvider>>()
                        .Where(rp => rp.GetType().GenericTypeArguments[0] == typeof(RsaEncryptorConfiguration))
                        .Select(p => new Tuple<IKeyManager, string, IEnumerable<IKey>>(p.KeyManager, p.Algorithm, p.GetAllKeys())),
                    p.GetRequiredService<IDefaultKeyResolver>(),
                    p.GetRequiredService<IProviderClient>()))
                .AddTransient(p => new KeyManagerWrapper<ECDsaEncryptorDescriptor>(p.GetService<IEnumerable<Aguacongas.IdentityServer.KeysRotation.ICacheableKeyRingProvider>>()
                        .Where(rp => rp.GetType().GenericTypeArguments[0] == typeof(ECDsaEncryptorConfiguration))
                        .Select(p => new Tuple<IKeyManager, string, IEnumerable<IKey>>(p.KeyManager, p.Algorithm, p.GetAllKeys())),
                    p.GetRequiredService<IDefaultKeyResolver>(),
                    p.GetRequiredService<IProviderClient>()))
                .AddSwaggerDocument(config =>
                {
                    config.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "Duende IdentityServer admin API";
                        document.Info.Description = "A web API to administrate your Duende IdentityServer";
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

                    ((NewtonsoftJsonSchemaGeneratorSettings)config.SchemaSettings).SerializerSettings = new JsonSerializerSettings
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
                            [apiName] = "Api access",
                            [SharedConstants.ADMINSCOPE] = "Admin scope access",
                            [SharedConstants.TOKENSCOPES] = "Token scope access"
                        },
                        Description = "Duende IdentityServer",
                        Name = "Duende IdentityServer",
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

            builder.AddApplicationPart(assembly)
                .AddApplicationPart(typeof(GenericApiController<>).Assembly)
                .ConfigureApplicationPartManager(apm =>
                    apm.FeatureProviders.Add(new GenericControllerFeatureProvider()));

            return CreateDynamicAuthenticationBuilder<TUser, TSchemeDefinition>(services);
        }

        private static DynamicAuthenticationBuilder CreateDynamicAuthenticationBuilder<TUser, TSchemeDefinition>(IServiceCollection services)
            where TUser : IdentityUser, new()
            where TSchemeDefinition : SchemeDefinitionBase, new()
        {
            var dynamicBuilder = services
                            .AddAuthentication()
                            .AddDynamic<TSchemeDefinition>();

            dynamicBuilder.AddGoogle(options =>
                {
                    options.Events = new OAuthEvents
                    {
                        OnTicketReceived = OnTicketReceived<TUser>()
                    };
                })
                .AddFacebook(options =>
                {
                    options.Events = new OAuthEvents
                    {
                        OnTicketReceived = OnTicketReceived<TUser>()
                    };
                })
                .AddTwitter(options =>
                {
                    options.Events = new TwitterEvents
                    {
                        OnTicketReceived = OnTicketReceived<TUser>()
                    };
                })
                .AddMicrosoftAccount(options =>
                {
                    options.Events = new OAuthEvents
                    {
                        OnTicketReceived = OnTicketReceived<TUser>()
                    };
                })
                .AddOpenIdConnect(options =>
                {
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTicketReceived = OnTicketReceived<TUser>()
                    };
                })
                .AddOAuth("OAuth", options =>
                {
                    options.Events = new OAuthEvents
                    {
                        OnTicketReceived = OnTicketReceived<TUser>(),
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
                            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("TheIdServer", "1.0.0"));

                            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted)
                                .ConfigureAwait(false);
                            var content = await response.Content.ReadAsStringAsync()
                                .ConfigureAwait(false);
                            response.EnsureSuccessStatusCode();

                            using var doc = JsonDocument.Parse(content);

                            context.RunClaimActions(doc.RootElement);
                        }
                    };
                })
                .AddWsFederation(options =>
                {
                    options.Events = new WsFederationEvents
                    {
                        OnTicketReceived = OnTicketReceived<TUser>()
                    };
                }).AddWindows(options =>
                {
                    options.Events = new NegotiateEvents
                    {
                        OnAuthenticated = async context =>
                        {
                            using var scope = context.HttpContext.RequestServices.CreateScope();
                            var transformer = scope.ServiceProvider.GetRequiredService<ExternalClaimsTransformer<TUser>>();
                            context.Principal = await transformer.TransformPrincipalAsync(context.Principal, context.Scheme.Name)
                                .ConfigureAwait(false);
                        }
                    };
                });

            return dynamicBuilder;
        }

        /// <summary>
        /// Adds the identity server admin filters.
        /// </summary>
        /// <param name="options">The options.</param>
        public static void AddIdentityServerAdminFilters(this MvcOptions options)
        {
            var filters = options.Filters;
            filters.Add<FormatFilter>();
            filters.Add<ExternalProviderFilter>();
            filters.Add<ExceptionFilter>();
            filters.Add<ActionsFilter>();

            options.FormatterMappings.SetMediaTypeMappingForFormat("export", JsonFileOutputFormatter.SupportedContentType);
            options.OutputFormatters.Add(new JsonFileOutputFormatter(options));
        }

        private static Func<TicketReceivedContext, Task> OnTicketReceived<TUser>()
            where TUser : IdentityUser, new()
        {
            return async context =>
            {
                using var scope = context.HttpContext.RequestServices.CreateScope();
                var transformer = scope.ServiceProvider.GetRequiredService<ExternalClaimsTransformer<TUser>>();
                context.Principal = await transformer.TransformPrincipalAsync(context.Principal, context.Scheme.Name)
                    .ConfigureAwait(false);
            };
        }
    }
}
