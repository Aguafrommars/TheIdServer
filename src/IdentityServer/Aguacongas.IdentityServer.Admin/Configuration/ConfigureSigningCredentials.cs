using Aguacongas.IdentityServer.Admin.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Aguacongas.IdentityServer.Admin.Configuration
{
    /// <summary>
    /// Configure signin credentials
    /// </summary>
    /// <seealso cref="IConfigureOptions{CredentialsOptions}" />
    public class ConfigureSigningCredentials : IConfigureOptions<CredentialsOptions>
    {
        // We need to cast the underlying int value of the EphemeralKeySet to X509KeyStorageFlags
        // due to the fact that is not part of .NET Standard. This value is only used with non-windows
        // platforms (all .NET Core) for which the value is defined on the underlying platform.
        private const X509KeyStorageFlags UnsafeEphemeralKeySet = (X509KeyStorageFlags)32;
        private const string DefaultTempKeyRelativePath = "obj/tempkey.json";
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigureSigningCredentials> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSigningCredentials"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public ConfigureSigningCredentials(
            IConfiguration configuration,
            ILogger<ConfigureSigningCredentials> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Invoked to configure a <typeparamref name="TOptions" /> instance.
        /// </summary>
        /// <param name="options">The options instance to configure.</param>
        public void Configure(CredentialsOptions options)
        {
            var key = LoadKey();
            options.SigningCredential = key;
        }

        /// <summary>
        /// Loads the key.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// Invalid certificate store location '{key.StoreLocation}'.
        /// or
        /// Key type not specified.
        /// or
        /// Invalid key type '{key.Kind?.ToString() ?? "(null)"}'.
        /// </exception>
        public SigningCredentials LoadKey()
        {
            var key = _configuration.Get<KeyDefinition>();
            
            switch (key.Type)
            {
                case KeyKinds.Development:
                    var developmentKeyPath = Path.Combine(Directory.GetCurrentDirectory(), key.FilePath ?? DefaultTempKeyRelativePath);
                    var createIfMissing = key.Persisted ?? true;
                    _logger.LogInformation($"Loading development key at '{developmentKeyPath}'.");
                    var developmentKey = new RsaSecurityKey(SigningKeysLoader.LoadDevelopment(developmentKeyPath, createIfMissing))
                    {
                        KeyId = "Development"
                    };
                    return new SigningCredentials(developmentKey, "RS256");
                case KeyKinds.File:
                    var pfxPath = Path.Combine(Directory.GetCurrentDirectory(), key.FilePath);
                    var storageFlags = GetStorageFlags(key);
                    _logger.LogInformation($"Loading certificate file at '{pfxPath}' with storage flags '{key.StorageFlags}'.");
                    return new SigningCredentials(new X509SecurityKey(SigningKeysLoader.LoadFromFile(pfxPath, key.Password, storageFlags)), "RS256");
                case KeyKinds.Store:
                    if (!Enum.TryParse<StoreLocation>(key.StoreLocation, out var storeLocation))
                    {
                        throw new InvalidOperationException($"Invalid certificate store location '{key.StoreLocation}'.");
                    }
                    _logger.LogInformation($"Loading certificate with subject '{key.Name}' in '{key.StoreLocation}\\{key.StoreName}'.");
                    return new SigningCredentials(new X509SecurityKey(SigningKeysLoader.LoadFromStoreCert(key.Name, key.StoreName, storeLocation, GetCurrentTime())), "RS256");
                case null:
                    throw new InvalidOperationException($"Key type not specified.");
                default:
                    throw new InvalidOperationException($"Invalid key type '{key.Type?.ToString() ?? "(null)"}'.");
            }
        }

        // for testing purposes only
        internal virtual DateTimeOffset GetCurrentTime() => DateTimeOffset.UtcNow;

        private X509KeyStorageFlags GetStorageFlags(KeyDefinition key)
        {
            var x509KeyStorageFlags = (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? X509KeyStorageFlags.PersistKeySet :
                            X509KeyStorageFlags.DefaultKeySet);
            var defaultFlags = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ?
                UnsafeEphemeralKeySet : x509KeyStorageFlags;

            if (key.StorageFlags == null)
            {
                return defaultFlags;
            }

            var flagsList = key.StorageFlags.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (flagsList.Length == 0)
            {
                return defaultFlags;
            }

            var result = ParseCurrentFlag(flagsList[0]);
            foreach (var flag in flagsList.Skip(1))
            {
                result |= ParseCurrentFlag(flag);
            }

            return result;

            static X509KeyStorageFlags ParseCurrentFlag(string candidate)
            {
                if (Enum.TryParse<X509KeyStorageFlags>(candidate, out var flag))
                {
                    return flag;
                }
                else
                {
                    throw new InvalidOperationException($"Invalid storage flag '{candidate}'");
                }
            }
        }
    }
}
