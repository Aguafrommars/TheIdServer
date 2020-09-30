using Microsoft.Extensions.DependencyInjection;
using System;

namespace Aguacongas.IdentityServer.KeysRotation.Extentions
{
    /// <summary>
    /// 
    /// </summary>
    public static class IIdentityServerBuilderExtensions
    {
        /// <summary>
        /// Adds the keys rotation.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureRsa">The configure RSA.</param>
        /// <returns></returns>
        public static IKeyRotationBuilder AddKeysRotation(this IIdentityServerBuilder builder, Action<KeyRotationOptions> configureKeysRotation = null)
        {
            return builder.Services.AddKeysRotation(configureKeysRotation);
        }
    }
}
