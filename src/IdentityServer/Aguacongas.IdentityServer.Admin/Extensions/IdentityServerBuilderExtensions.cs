using Aguacongas.IdentityServer.Admin.Configuration;
using Aguacongas.IdentityServer.Admin.Options;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IIdentityServerBuilder"/> extnesions
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Adds a signing key.
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <returns>The <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddSigningCredentials(this IIdentityServerBuilder builder)
        {
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IConfigureOptions<CredentialsOptions>, ConfigureSigningCredentials>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<ConfigureSigningCredentials>>();
                    var effectiveConfig = sp.GetRequiredService<IConfiguration>().GetSection("IdentityServer:Key");
                    return new ConfigureSigningCredentials(effectiveConfig, logger);
                }));

            // We take over the setup for the credentials store as Identity Server registers a singleton
            builder.Services.AddSingleton<ISigningCredentialStore>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<CredentialsOptions>>();
                return new InMemorySigningCredentialsStore(options.Value.SigningCredential);
            });

            // We take over the setup for the validation keys store as Identity Server registers a singleton
            builder.Services.AddSingleton<IValidationKeysStore>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<CredentialsOptions>>();
                return new InMemoryValidationKeysStore(new[]
                {
                    new SecurityKeyInfo
                    {
                        Key = options.Value.SigningCredential.Key,
                        SigningAlgorithm = options.Value.SigningCredential.Algorithm
                    }
                });
            });

            return builder;
        }

        /// <summary>
        /// Adds the dynamic client registration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddDynamicClientRegistration(this IIdentityServerBuilder builder)
        {
            var services = builder.Services;
            services.Configure<IdentityServerOptions>(option =>
            {
                var discovery = option.Discovery;
                discovery.ExpandRelativePathsInCustomEntries = true;
                discovery.CustomEntries.Add("registration_endpoint", "~/api/register");
            });
            return builder;
        }
    }
}
