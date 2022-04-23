// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public sealed class RsaEncryptorFactory : IAuthenticatedEncryptorFactory
    {
        private readonly ILogger _logger;

        public RsaEncryptorFactory(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RsaEncryptorFactory>();
        }

        public IAuthenticatedEncryptor CreateEncryptorInstance(IKey key)
        {
            if (!(key.Descriptor is RsaEncryptorDescriptor descriptor))
            {
                return null;
            }

            return CreateAuthenticatedEncryptorInstance(descriptor.RsaSecurityKey, descriptor.Configuration);
        }

        internal RsaEncryptor CreateAuthenticatedEncryptorInstance(
            RsaSecurityKey secret,
            RsaEncryptorConfiguration configuration)
        {
            if (configuration == null)
            {
                return null;
            }

            _logger.LogDebug($"Create new {nameof(RsaEncryptor)}");
            return new RsaEncryptor(secret);
        }
    }
}
