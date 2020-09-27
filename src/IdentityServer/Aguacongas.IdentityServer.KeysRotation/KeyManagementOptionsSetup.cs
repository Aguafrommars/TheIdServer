using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Aguacongas.IdentityServer.KeysRotation
{
    internal class KeyManagementOptionsSetup : IConfigureOptions<KeyManagementOptions>
    {
        private readonly ILoggerFactory _loggerFactory;

        public KeyManagementOptionsSetup()
            : this(NullLoggerFactory.Instance)
        {
        }

        public KeyManagementOptionsSetup(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Configure(KeyManagementOptions options)
        {
            options.AuthenticatedEncryptorFactories.Add(new RsaEncryptorFactory(_loggerFactory));
        }
    }
}
