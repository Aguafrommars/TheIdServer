// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.Extensions.DependencyInjection;
using System;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Aguacongas.IdentityServer.KeysRotation.Extensions
{
    /// <summary>
    /// <see cref="IIdentityServerBuilder"/> extensions
    /// </summary>
    public static class IIdentityServerBuilderExtensions
    {
        /// <summary>
        /// Adds the keys rotation.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureRsa">The configure RSA.</param>
        /// <returns></returns>
        public static IKeyRotationBuilder AddKeysRotation(this IIdentityServerBuilder builder, RsaSigningAlgorithm rsaSigningAlgorithm = RsaSigningAlgorithm.RS256, Action<KeyRotationOptions> configureKeysRotation = null)
        {
            return builder.Services.AddKeysRotation(rsaSigningAlgorithm, configureKeysRotation);
        }

        /// <summary>
        /// Adds Rsa keys rotation.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureRsa">The configure RSA.</param>
        /// <returns></returns>
        public static IKeyRotationBuilder AddRsaKeysRotation(this IKeyRotationBuilder builder, RsaSigningAlgorithm signingAlgorithm, Action<KeyRotationOptions> configureKeysRotation = null)
        {
            builder.Services.AddKeysRotation<RsaEncryptorConfiguration, RsaEncryptor>(signingAlgorithm.ToString(), configureKeysRotation);
            return builder;
        }

        /// <summary>
        /// Adds ECDsa keys rotation.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureRsa">The configure RSA.</param>
        /// <returns></returns>
        public static IKeyRotationBuilder AddECDsaKeysRotation(this IKeyRotationBuilder builder, ECDsaSigningAlgorithm signingAlgorithm, Action<KeyRotationOptions> configureKeysRotation = null)
        {
            builder.Services.AddKeysRotation<ECDsaEncryptorConfiguration, ECDsaEncryptor>(signingAlgorithm.ToString(), configureKeysRotation);
            return builder;
        }
    }
}
