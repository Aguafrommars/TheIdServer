// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public sealed class ECDsaEncryptorFactory : IAuthenticatedEncryptorFactory
    {
        private readonly ILogger _logger;

        public ECDsaEncryptorFactory(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ECDsaEncryptorFactory>();
        }

        public IAuthenticatedEncryptor CreateEncryptorInstance(IKey key)
        {
            if (!(key.Descriptor is ECDsaEncryptorDescriptor descriptor))
            {
                return null;
            }

            return CreateAuthenticatedEncryptorInstance(descriptor.ECDsaSecurityKey, descriptor.Configuration);
        }

        internal ECDsaEncryptor CreateAuthenticatedEncryptorInstance(
            ECDsaSecurityKey secret,
            SigningAlgorithmConfiguration configuration)
        {
            if (configuration == null)
            {
                return null;
            }

            _logger.LogDebug($"Create new {nameof(ECDsaEncryptor)}");
            return new ECDsaEncryptor(secret);
        }
    }
}
