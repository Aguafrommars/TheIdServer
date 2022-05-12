// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.KeysRotation.AzureKeyVault;
using Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore;
using Aguacongas.IdentityServer.KeysRotation.XmlEncryption;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.Azure.KeyVault;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using StackExchange.Redis;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Raven.Client.Documents.Session;
using Raven.Client.Documents;
using mongoDb = Aguacongas.IdentityServer.KeysRotation.MongoDb;
using MongoDB.Driver;
#if DUENDE
using static Duende.IdentityServer.IdentityServerConstants;
#else
using static IdentityServer4.IdentityServerConstants;
#endif
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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (blobSasUri == null)
            {
                throw new ArgumentNullException(nameof(blobSasUri));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (blobUri == null)
            {
                throw new ArgumentNullException(nameof(blobUri));
            }
            if (tokenCredential == null)
            {
                throw new ArgumentNullException(nameof(tokenCredential));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (blobUri == null)
            {
                throw new ArgumentNullException(nameof(blobUri));
            }
            if (sharedKeyCredential == null)
            {
                throw new ArgumentNullException(nameof(sharedKeyCredential));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            if (containerName == null)
            {
                throw new ArgumentNullException(nameof(containerName));
            }
            if (blobName == null)
            {
                throw new ArgumentNullException(nameof(blobName));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (blobClient == null)
            {
                throw new ArgumentNullException(nameof(blobClient));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (getSession == null)
            {
                getSession = p => {
                    var store = p.GetRequiredService<IDocumentStore>();
                    return store.OpenSession();
                };
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (getCollection == null)
            {
                getCollection = p => p.GetRequiredService<IMongoDatabase>().GetCollection<mongoDb.KeyRotationKey>(nameof(mongoDb.KeyRotationKey));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (databaseFactory == null)
            {
                throw new ArgumentNullException(nameof(databaseFactory));
            }
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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (connectionMultiplexer == null)
            {
                throw new ArgumentNullException(nameof(connectionMultiplexer));
            }
            return PersistKeysToStackExchangeRedisInternal(builder, () => connectionMultiplexer.GetDatabase(), key);
        }

        /// <summary>
        /// Configures keys to be encrypted to a given certificate before being persisted to storage.
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder"/>.</param>
        /// <param name="certificate">The certificate to use when encrypting keys.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IKeyRotationBuilder ProtectKeysWithCertificate(this IKeyRotationBuilder builder, X509Certificate2 certificate)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (thumbprint == null)
            {
                throw new ArgumentNullException(nameof(thumbprint));
            }

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
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyIdentifier">The Azure KeyVault key identifier used for key encryption.</param>
        /// <param name="clientId">The application client id.</param>
        /// <param name="certificate"></param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(this IKeyRotationBuilder builder, string keyIdentifier, string clientId, X509Certificate2 certificate)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            Task<string> callback(string authority, string resource, string scope) => GetTokenFromClientCertificateAsync(authority, resource, clientId, certificate);

            return ProtectKeysWithAzureKeyVault(builder, new KeyVaultClient(callback), keyIdentifier);
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with specified key in Azure KeyVault.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="keyIdentifier">The Azure KeyVault key identifier used for key encryption.</param>
        /// <param name="clientId">The application client id.</param>
        /// <param name="clientSecret">The client secret to use for authentication.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(this IKeyRotationBuilder builder, string keyIdentifier, string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            Task<string> callback(string authority, string resource, string scope) => GetTokenFromClientSecretAsync(authority, resource, clientId, clientSecret);

            return ProtectKeysWithAzureKeyVault(builder, new KeyVaultClient(callback), keyIdentifier);
        }

        /// <summary>
        /// Configures the key rotation system to protect keys with specified key in Azure KeyVault.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="client">The <see cref="KeyVaultClient"/> to use for KeyVault access.</param>
        /// <param name="keyIdentifier">The Azure KeyVault key identifier used for key encryption.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IKeyRotationBuilder ProtectKeysWithAzureKeyVault(this IKeyRotationBuilder builder, KeyVaultClient client, string keyIdentifier)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrEmpty(keyIdentifier))
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }

            var vaultClientWrapper = new KeyVaultClientWrapper(client);

            builder.Services.AddSingleton<IKeyVaultWrappingClient>(vaultClientWrapper);
            builder.Services.Configure<KeyRotationOptions>(options =>
            {
                options.XmlEncryptor = new AzureKeyVaultXmlEncryptor(vaultClientWrapper, keyIdentifier);
            });

            return builder;
        }
        private static async Task<string> GetTokenFromClientCertificateAsync(string authority, string resource, string clientId, X509Certificate2 certificate)
        {
            var authContext = new AuthenticationContext(authority);
            var result = await authContext.AcquireTokenAsync(resource, new ClientAssertionCertificate(clientId, certificate)).ConfigureAwait(false);
            return result.AccessToken;
        }

        private static async Task<string> GetTokenFromClientSecretAsync(string authority, string resource, string clientId, string clientSecret)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(clientId, clientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred).ConfigureAwait(false);
            return result.AccessToken;
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
