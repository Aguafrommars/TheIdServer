// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
// Migration to Azure.Security.KeyVault SDK

using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.KeysRotation.AzureKeyVault;
using Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore;
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Aguacongas.IdentityServer.KeysRotation.XmlEncryption;
using Azure.Core;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using StackExchange.Redis;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using static Duende.IdentityServer.IdentityServerConstants;
using mongoDb = Aguacongas.IdentityServer.KeysRotation.MongoDb;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KeyRotationBuilderExtensions
    {
        /// <summary>
        /// Configures the key management options for the key rotation system.
        /// </summary>
        /// <param name="builder">The <see cref="IKeyRotationBuilder"/>.</param>
        /// <param name="setupAction">An <see cref="Action{ECDsaEncryptorConfiguration}"/> to configure the provided <see cref="ECDsaEncryptorConfiguration"/>.</param>
        /// <param name="signingAlgorithm">The siging algorithm</param>
        /// <returns>A reference to the <see cref="IKeyRotationBuilder" /> after this operation has completed.</returns>
        public static IKeyRotationBuilder AddECDsaEncryptorConfiguration(this IKeyRotationBuilder builder, ECDsaSigningAlgorithm signingAlgorithm, Action<ECDsaEncryptorConfiguration> setupAction)
        {
            ArgumentNullException.ThrowIfNull(builder);

            ArgumentNullException.ThrowIfNull(setupAction);

            builder.Services.AddSingleton<IConfigureOptions<KeyRotationOptions>>(services =>
            {
                return new ConfigureOptions<KeyRotationOptions>(options =>
                {
                    if (options.AuthenticatedEncryptorConfiguration is ECDsaEncryptorConfiguration configuration && configuration.SigningAlgorithm == signingAlgorithm.ToString())
                    {
                        setupAction(configuration);
                    }
                });
            });
            return builder;
        }

        /// <summary>
        /// Configures the key management options for the key rotation system.
        /// </summary>
        /// <param name="builder">The <see cref="IKeyRotationBuilder"/>.</param>
        /// <param name="setupAction">An <see cref="Action{RsaEncryptorConfiguration}"/> to configure the provided <see cref="RsaEncryptorConfiguration"/>.</param>
        /// <returns>A reference to the <see cref="IKeyRotationBuilder" /> after this operation has completed.</returns>
        public static IKeyRotationBuilder AddRsaEncryptorConfiguration(this IKeyRotationBuilder builder, RsaSigningAlgorithm signingAlgorithm, Action<RsaEncryptorConfiguration> setupAction)
        {
            ArgumentNullException.ThrowIfNull(builder);

            ArgumentNullException.ThrowIfNull(setupAction);

            builder.Services.AddSingleton<IConfigureOptions<KeyRotationOptions>>(services =>
            {
                return new ConfigureOptions<KeyRotationOptions>(options =>
                {
                    if (options.AuthenticatedEncryptorConfiguration is RsaEncryptorConfiguration configuration && configuration.SigningAlgorithm == signingAlgorithm.ToString())
                    {
                        setupAction(configuration);
                    }
                });
            });
            return builder;
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified path
        /// in Azure Blob Storage.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="sasUri">The full URI where the key file should be stored.
        /// The URI must contain the SAS token as a query string parameter.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        /// <remarks>
        /// The container referenced by <paramref name="blobSasUri"/> must already exist.
        /// </remarks>
        public static IKeyRotationBuilder PersistKeysToAzureBlobStorage(this IKeyRotationBuilder builder, Uri blobSasUri)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(blobSasUri);

            var uriBuilder = new BlobUriBuilder(blobSasUri);
            BlobClient client;

            // The SAS token is present in the query string.
            if (uriBuilder.Sas == null)
            {
                throw new ArgumentException($"{nameof(blobSasUri)} is expected to be a SAS URL.", nameof(blobSasUri));
            }
            else
            {
                client = new BlobClient(blobSasUri);
            }

            return PersistKeysToAzureBlobStorage(builder, client);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified path
        /// in Azure Blob Storage.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="sasUri">The full URI where the key file should be stored.
        /// The URI must contain the SAS token as a query string parameter.</param>
        /// <param name="tokenCredential">The credentials to connect to the blob.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        /// <remarks>
        /// The container referenced by <paramref name="blobUri"/> must already exist.
        /// </remarks>
        public static IKeyRotationBuilder PersistKeysToAzureBlobStorage(this IKeyRotationBuilder builder, Uri blobUri, TokenCredential tokenCredential)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(blobUri);
            ArgumentNullException.ThrowIfNull(tokenCredential);

            var client = new BlobClient(blobUri, tokenCredential);

            return PersistKeysToAzureBlobStorage(builder, client);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified path
        /// in Azure Blob Storage.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="blobUri">The full URI where the key file should be stored.
        /// The URI must contain the SAS token as a query string parameter.</param>
        /// <param name="sharedKeyCredential">The credentials to connect to the blob.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        /// <remarks>
        /// The container referenced by <paramref name="blobUri"/> must already exist.
        /// </remarks>
        public static IKeyRotationBuilder PersistKeysToAzureBlobStorage(this IKeyRotationBuilder builder, Uri blobUri, StorageSharedKeyCredential sharedKeyCredential)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(blobUri);
            ArgumentNullException.ThrowIfNull(sharedKeyCredential);

            var client = new BlobClient(blobUri, sharedKeyCredential);

            return PersistKeysToAzureBlobStorage(builder, client);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified path
        /// in Azure Blob Storage.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="connectionString">A connection string includes the authentication information
        /// required for your application to access data in an Azure Storage
        /// account at runtime.
        /// </param>
        /// <param name="containerName">The container name to use.</param>
        /// <param name="blobName">The blob name to use.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        /// <remarks>
        /// The container referenced by <paramref name="containerName"/><paramref name="blobName"/> must already exist.
        /// </remarks>
        public static IKeyRotationBuilder PersistKeysToAzureBlobStorage(this IKeyRotationBuilder builder, string connectionString, string containerName, string blobName)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(connectionString);
            ArgumentNullException.ThrowIfNull(containerName);
            ArgumentNullException.ThrowIfNull(blobName);

            var client = new BlobServiceClient(connectionString).GetBlobContainerClient(containerName).GetBlobClient(blobName);

            return PersistKeysToAzureBlobStorage(builder, client);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified path
        /// in Azure Blob Storage.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="blobClient">The <see cref="BlobClient"/> in which the
        /// key file should be stored.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        /// <remarks>
        /// The blob referenced by <paramref name="blobClient"/> must already exist.
        /// </remarks>
        public static IKeyRotationBuilder PersistKeysToAzureBlobStorage(this IKeyRotationBuilder builder, BlobClient blobClient)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(blobClient);

            builder.Services.Configure<KeyRotationOptions>(options =>
            {
                options.XmlRepository = new AzureBlobXmlRepository(blobClient);
            });
            return builder;
        }

        /// <summary>
        /// Configures the key rotation system to persist keys to an EntityFrameworkCore datastore
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder"/> instance to modify.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder PersistKeysToDbContext<TContext>(this IKeyRotationBuilder builder) where TContext : DbContext, IKeyRotationContext
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Services.AddSingleton<IConfigureOptions<KeyRotationOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyRotationOptions>(options =>
                {
                    options.XmlRepository = new EntityFrameworkCoreXmlRepository<TContext>(services, loggerFactory);
                });
            });
            return builder;
        }

        /// <summary>
        /// Configures the key rotation system to persist keys to a RavenDb datasstore
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder" /> instance to modify.</param>
        /// <param name="getSession">The get session.</param>
        /// <returns>
        /// The value <paramref name="builder" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder</exception>
        public static IKeyRotationBuilder PersistKeysToRavenDb(this IKeyRotationBuilder builder, Func<IServiceProvider, IDocumentSession> getSession = null)
        {
            ArgumentNullException.ThrowIfNull(builder);

            getSession ??= p => {
                    var store = p.GetRequiredService<IDocumentStore>();
                    return store.OpenSession();
                };

            builder.Services.AddSingleton<IConfigureOptions<KeyRotationOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyRotationOptions>(options =>
                {
                    options.XmlRepository = new RavenDbXmlRepository<Aguacongas.IdentityServer.KeysRotation.RavenDb.KeyRotationKey>(services, loggerFactory);
                });
            })
                .AddTransient(p => new DocumentSessionWrapper(getSession(p)));

            return builder;
        }

        /// <summary>
        /// Configures the key rotation system to persist keys to a MongoDb datasstore
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder" /> instance to modify.</param>
        /// <param name="getSession">The get session.</param>
        /// <returns>
        /// The value <paramref name="builder" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder</exception>
        public static IKeyRotationBuilder PersistKeysToMongoDb(this IKeyRotationBuilder builder, Func<IServiceProvider, IMongoCollection<mongoDb.KeyRotationKey>> getCollection = null)
        {
            ArgumentNullException.ThrowIfNull(builder);

            getCollection ??= p => p.GetRequiredService<IMongoDatabase>().GetCollection<mongoDb.KeyRotationKey>(nameof(mongoDb.KeyRotationKey));

            builder.Services.AddSingleton<IConfigureOptions<KeyRotationOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyRotationOptions>(options =>
                {
                    options.XmlRepository = new mongoDb.MongoDbXmlRepository<mongoDb.KeyRotationKey>(services, loggerFactory);
                });
            })
                .AddTransient(p => new mongoDb.MongoCollectionWrapper<mongoDb.KeyRotationKey>(getCollection(p)));

            return builder;
        }

        /// <summary>
        /// Configures the key rotation system to persist keys to the specified directory.
        /// This path may be on the local machine or may point to a UNC share.
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder"/>.</param>
        /// <param name="directory">The directory in which to store keys.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IKeyRotationBuilder PersistKeysToFileSystem(this IKeyRotationBuilder builder, DirectoryInfo directory)
        {
            ArgumentNullException.ThrowIfNull(builder);

            ArgumentNullException.ThrowIfNull(directory);

            builder.Services.AddSingleton<IConfigureOptions<KeyRotationOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyRotationOptions>(options =>
                {
                    options.XmlRepository = new FileSystemXmlRepository(directory, loggerFactory);
                });
            });
            return builder;
        }

        private const string KeyRotationKeysName = "KeyRotation-Keys";

        /// <summary>
        /// Configures the key rotation system to persist keys to specified key in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="databaseFactory">The delegate used to create <see cref="IDatabase"/> instances.</param>
        /// <param name="key">The <see cref="RedisKey"/> used to store key list.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IKeyRotationBuilder PersistKeysToStackExchangeRedis(this IKeyRotationBuilder builder, Func<IDatabase> databaseFactory, RedisKey key)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(databaseFactory);
            return PersistKeysToStackExchangeRedisInternal(builder, databaseFactory, key);
        }

        /// <summary>
        /// Configures the key rotation system to persist keys to the default key ('DataProtection-Keys') in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="connectionMultiplexer">The <see cref="IConnectionMultiplexer"/> for database access.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IKeyRotationBuilder PersistKeysToStackExchangeRedis(this IKeyRotationBuilder builder, IConnectionMultiplexer connectionMultiplexer)
        {
            return PersistKeysToStackExchangeRedis(builder, connectionMultiplexer, KeyRotationKeysName);
        }
        /// <summary>
        /// Configures the key rotation system to persist keys to the specified key in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="connectionMultiplexer">The <see cref="IConnectionMultiplexer"/> for database access.</param>
        /// <param name="key">The <see cref="RedisKey"/> used to store key list.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IKeyRotationBuilder PersistKeysToStackExchangeRedis(this IKeyRotationBuilder builder, IConnectionMultiplexer connectionMultiplexer, RedisKey key)
        {
            ArgumentNullException.ThrowIfNull(builder);
            return connectionMultiplexer == null
                ? throw new ArgumentNullException(nameof(connectionMultiplexer))
                : PersistKeysToStackExchangeRedisInternal(builder, () => connectionMultiplexer.GetDatabase(), key);
        }

        /// <summary>
        /// Configures keys to be encrypted to a given certificate before being persisted to storage.
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder"/>.</param>
        /// <param name="certificate">The certificate to use when encrypting keys.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IKeyRotationBuilder ProtectKeysWithCertificate(this IKeyRotationBuilder builder, X509Certificate2 certificate)
        {
            ArgumentNullException.ThrowIfNull(builder);

            ArgumentNullException.ThrowIfNull(certificate);

            builder.Services.AddSingleton<IConfigureOptions<KeyRotationOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyRotationOptions>(options =>
                {
                    options.XmlEncryptor = new CertificateXmlEncryptor(certificate, loggerFactory);
                });
            });

            builder.Services.Configure<XmlKeyDecryptionOptions>(o => o.AddKeyDecryptionCertificate(certificate));

            return builder;
        }

        /// <summary>
        /// Configures keys to be encrypted to a given certificate before being persisted to storage.
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder"/>.</param>
        /// <param name="thumbprint">The thumbprint of the certificate to use when encrypting keys.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IKeyRotationBuilder ProtectKeysWithCertificate(this IKeyRotationBuilder builder, string thumbprint)
        {
            ArgumentNullException.ThrowIfNull(builder);

            ArgumentNullException.ThrowIfNull(thumbprint);

            // Make sure the thumbprint corresponds to a valid certificate.
            if (new AspNetCore.DataProtection.XmlEncryption.CertificateResolver().ResolveCertificate(thumbprint) == null)
            {
                throw new InvalidOperationException($"A certificate with the thumbprint '{thumbprint}' could not be found.");
            }

            // ICertificateResolver is necessary for this type to work correctly, so register it
            // if it doesn't already exist.
            builder.Services.TryAddSingleton<AspNetCore.DataProtection.XmlEncryption.ICertificateResolver, AspNetCore.DataProtection.XmlEncryption.CertificateResolver>();

            builder.Services.AddSingleton<IConfigureOptions<KeyRotationOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                var certificateResolver = services.GetRequiredService<AspNetCore.DataProtection.XmlEncryption.ICertificateResolver>();
                return new ConfigureOptions<KeyRotationOptions>(options =>
                {
                    options.XmlEncryptor = new CertificateXmlEncryptor(thumbprint, certificateResolver, loggerFactory);
                });
            });

            return builder;
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with specified key in Azure KeyVault.
        /// Uses DefaultAzureCredential for authentication.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyVaultUri">The Azure KeyVault URI (e.g., https://your-vault.vault.azure.net/)</param>
        /// <param name="keyName">The name of the key to use for encryption</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(
            this IKeyRotationBuilder builder,
            string keyVaultUri,
            string keyName)
        {
            ArgumentNullException.ThrowIfNull(builder);
            if (string.IsNullOrEmpty(keyVaultUri))
            {
                throw new ArgumentNullException(nameof(keyVaultUri));
            }
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentNullException(nameof(keyName));
            }

            var vaultUri = new Uri(keyVaultUri);
            var credential = new DefaultAzureCredential();

            return ProtectKeysWithAzureKeyVault(builder, vaultUri, keyName, credential);
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with specified key in Azure KeyVault.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyVaultUri">The Azure KeyVault URI</param>
        /// <param name="keyName">The name of the key to use for encryption</param>
        /// <param name="credential">The credential to use for authentication</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(
            this IKeyRotationBuilder builder,
            Uri keyVaultUri,
            string keyName,
            TokenCredential credential)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(keyVaultUri);
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentNullException(nameof(keyName));
            }
            ArgumentNullException.ThrowIfNull(credential);

            // Build the full key identifier
            var keyIdentifier = $"{keyVaultUri.AbsoluteUri.TrimEnd('/')}/keys/{keyName}";

            return ProtectKeysWithAzureKeyVault(builder, keyIdentifier, credential);
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with specified key in Azure KeyVault.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyIdentifier">The full Azure KeyVault key identifier (URI)</param>
        /// <param name="credential">The credential to use for authentication</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(
            this IKeyRotationBuilder builder,
            string keyIdentifier,
            TokenCredential credential)
        {
            ArgumentNullException.ThrowIfNull(builder);
            if (string.IsNullOrEmpty(keyIdentifier))
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }
            ArgumentNullException.ThrowIfNull(credential);

            // Extract vault URI from key identifier to create KeyClient
            Uri keyUri;
            try
            {
                keyUri = new Uri(keyIdentifier);
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException($"Invalid key identifier format: {keyIdentifier}", nameof(keyIdentifier), ex);
            }

            var vaultUri = new Uri($"{keyUri.Scheme}://{keyUri.Host}");

            // Register KeyVault client wrapper
            builder.Services.AddSingleton<IKeyVaultWrappingClient>(sp =>
            {
                var logger = sp.GetService<ILogger<KeyVaultClientWrapper>>();
                logger?.LogInformation(
                    "Initializing Azure KeyVault client for vault: {VaultUri}, key: {KeyIdentifier}",
                    vaultUri, keyIdentifier);

                return new KeyVaultClientWrapper(vaultUri, credential);
            });

            // Configure XML encryption
            builder.Services.AddSingleton<IConfigureOptions<KeyRotationOptions>>(services =>
            {
                var keyVaultClient = services.GetRequiredService<IKeyVaultWrappingClient>();
                return new ConfigureOptions<KeyRotationOptions>(options =>
                {
                    options.XmlEncryptor = new AzureKeyVaultXmlEncryptor(keyVaultClient, keyIdentifier);
                });
            });

            return builder;
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with Azure KeyVault using a client secret.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyVaultUri">The Azure KeyVault URI</param>
        /// <param name="keyName">The name of the key to use for encryption</param>
        /// <param name="tenantId">The Azure AD tenant ID</param>
        /// <param name="clientId">The application client ID</param>
        /// <param name="clientSecret">The client secret</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(
            this IKeyRotationBuilder builder,
            string keyVaultUri,
            string keyName,
            string tenantId,
            string clientId,
            string clientSecret)
        {
            ArgumentNullException.ThrowIfNull(builder);
            if (string.IsNullOrEmpty(keyVaultUri))
            {
                throw new ArgumentNullException(nameof(keyVaultUri));
            }
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentNullException(nameof(keyName));
            }
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            return ProtectKeysWithAzureKeyVault(builder, new Uri(keyVaultUri), keyName, credential);
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with Azure KeyVault using a certificate.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyVaultUri">The Azure KeyVault URI</param>
        /// <param name="keyName">The name of the key to use for encryption</param>
        /// <param name="tenantId">The Azure AD tenant ID</param>
        /// <param name="clientId">The application client ID</param>
        /// <param name="certificate">The client certificate for authentication</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(
            this IKeyRotationBuilder builder,
            string keyVaultUri,
            string keyName,
            string tenantId,
            string clientId,
            X509Certificate2 certificate)
        {
            ArgumentNullException.ThrowIfNull(builder);
            if (string.IsNullOrEmpty(keyVaultUri))
            {
                throw new ArgumentNullException(nameof(keyVaultUri));
            }
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentNullException(nameof(keyName));
            }
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            ArgumentNullException.ThrowIfNull(certificate);

            var credential = new ClientCertificateCredential(tenantId, clientId, certificate);
            return ProtectKeysWithAzureKeyVault(builder, new Uri(keyVaultUri), keyName, credential);
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with specified key in Azure KeyVault.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyIdentifier">The Azure KeyVault key identifier used for key encryption.</param>
        /// <param name="clientId">The application client id.</param>
        /// <param name="certificate">The client certificate for authentication.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        [Obsolete("This method uses the legacy Azure KeyVault SDK. Use ProtectKeysWithAzureKeyVault(builder, keyVaultUri, keyName, tenantId, clientId, certificate) instead.", error: false)]
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(
            this IKeyRotationBuilder builder,
            string keyIdentifier,
            string clientId,
            X509Certificate2 certificate)
        {
            ArgumentNullException.ThrowIfNull(builder);
            if (string.IsNullOrEmpty(keyIdentifier))
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            ArgumentNullException.ThrowIfNull(certificate);

            // For backward compatibility, try to extract tenant from key identifier or use common
            // This is a best-effort migration path
            var tenantId = "common"; // Default to common tenant

            // Extract vault URI and key name from identifier
            var keyUri = new Uri(keyIdentifier);
            var vaultUri = $"{keyUri.Scheme}://{keyUri.Host}";
            var pathParts = keyUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var keyName = pathParts.Length >= 2 ? pathParts[1] : throw new ArgumentException("Invalid key identifier format", nameof(keyIdentifier));

            var logger = NullLoggerFactory.Instance.CreateLogger<KeyVaultClientWrapper>();
            logger.LogWarning(
                "Using obsolete ProtectKeysWithAzureKeyVault method. Please update to use the new method with explicit tenant ID. " +
                "Using 'common' as tenant ID for authentication.");

            return ProtectKeysWithAzureKeyVault(builder, vaultUri, keyName, tenantId, clientId, certificate);
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with specified key in Azure KeyVault.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyIdentifier">The Azure KeyVault key identifier used for key encryption.</param>
        /// <param name="clientId">The application client id.</param>
        /// <param name="clientSecret">The client secret to use for authentication.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        [Obsolete("This method uses the legacy Azure KeyVault SDK. Use ProtectKeysWithAzureKeyVault(builder, keyVaultUri, keyName, tenantId, clientId, clientSecret) instead.", error: false)]
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(
            this IKeyRotationBuilder builder,
            string keyIdentifier,
            string clientId,
            string clientSecret)
        {
            ArgumentNullException.ThrowIfNull(builder);
            if (string.IsNullOrEmpty(keyIdentifier))
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            // For backward compatibility, try to extract tenant from key identifier or use common
            var tenantId = "common"; // Default to common tenant

            // Extract vault URI and key name from identifier
            var keyUri = new Uri(keyIdentifier);
            var vaultUri = $"{keyUri.Scheme}://{keyUri.Host}";
            var pathParts = keyUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var keyName = pathParts.Length >= 2 ? pathParts[1] : throw new ArgumentException("Invalid key identifier format", nameof(keyIdentifier));

            var logger = NullLoggerFactory.Instance.CreateLogger<KeyVaultClientWrapper>();
            logger.LogWarning(
                "Using obsolete ProtectKeysWithAzureKeyVault method. Please update to use the new method with explicit tenant ID. " +
                "Using 'common' as tenant ID for authentication.");

            return ProtectKeysWithAzureKeyVault(builder, vaultUri, keyName, tenantId, clientId, clientSecret);
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with Azure KeyVault using Managed Identity.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyVaultUri">The Azure KeyVault URI</param>
        /// <param name="keyName">The name of the key to use for encryption</param>
        /// <param name="managedIdentityClientId">Optional client ID of the user-assigned managed identity</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVaultManagedIdentity(
            this IKeyRotationBuilder builder,
            string keyVaultUri,
            string keyName,
            string managedIdentityClientId = null)
        {
            ArgumentNullException.ThrowIfNull(builder);
            if (string.IsNullOrEmpty(keyVaultUri))
            {
                throw new ArgumentNullException(nameof(keyVaultUri));
            }
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentNullException(nameof(keyName));
            }

            var credential = string.IsNullOrEmpty(managedIdentityClientId)
                ? new ManagedIdentityCredential()
                : new ManagedIdentityCredential(managedIdentityClientId);

            return ProtectKeysWithAzureKeyVault(builder, new Uri(keyVaultUri), keyName, credential);
        }

        private static IKeyRotationBuilder PersistKeysToStackExchangeRedisInternal(IKeyRotationBuilder builder, Func<IDatabase> databaseFactory, RedisKey key)
        {
            builder.Services.Configure<KeyRotationOptions>(options =>
            {
                options.XmlRepository = new RedisXmlRepository(databaseFactory, key);
            });
            return builder;
        }
    }
}