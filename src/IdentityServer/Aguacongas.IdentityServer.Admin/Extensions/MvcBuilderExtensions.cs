using Aguacongas.IdentityServer.Admin;
using Aguacongas.IdentityServer.Admin.Filters;
using Aguacongas.IdentityServer.Admin.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        public static IMvcBuilder AddIdentityServerAdmin<TUser>(this IMvcBuilder builder)
        {
            var assembly = typeof(MvcBuilderExtensions).Assembly;
            builder.Services.AddTransient<IPersistedGrantService, PersistedGrantService>()
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
                            Email = string.Empty,
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
            filters.Add(new SelectFilter());
            filters.Add(new ExceptionFilter());
            filters.Add(new ActionsFilter());
        }
    }
}
