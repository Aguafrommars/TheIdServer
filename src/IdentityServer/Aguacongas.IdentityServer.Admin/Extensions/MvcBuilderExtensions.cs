using Aguacongas.IdentityServer.Admin;
using Aguacongas.IdentityServer.Admin.Services;
using IdentityServer4.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddIdentityServerAdmin(this IMvcBuilder builder)
        {
            var assembly = typeof(MvcBuilderExtensions).Assembly;
            builder.Services.AddTransient<IPersistedGrantService, PersistedGrantService>();
            return builder.AddApplicationPart(assembly)
                .ConfigureApplicationPartManager(apm =>
                    apm.FeatureProviders.Add(new GenericApiControllerFeatureProvider()));        }
    }
}
