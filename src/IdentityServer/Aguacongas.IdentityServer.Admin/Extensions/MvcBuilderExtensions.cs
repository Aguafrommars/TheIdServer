using Aguacongas.IdentityServer.Admin.Service;
using IdentityServer4.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddIdentityServerAdmin(this IMvcBuilder builder)
        {
            var assembly = typeof(MvcBuilderExtensions).Assembly;
            builder.Services.AddTransient<IPersistedGrantService, PersistedGrantService>();
            return builder.AddApplicationPart(assembly); 
        }
    }
}
