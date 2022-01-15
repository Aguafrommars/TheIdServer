using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddCiba(this IIdentityServerBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddCibaServices(configuration);
            return builder;
        }
    }
}
