using Aguacongas.IdentityServer.KeysRotation;
using Microsoft.AspNetCore.DataProtection.AzureStorage;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KeyRotationBuilderExtensions
    {
        /// <summary>
        /// Configures the data protection system to persist keys to the specified path
        /// in Azure Blob Storage.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="storageAccount">The <see cref="CloudStorageAccount"/> which
        /// should be utilized.</param>
        /// <param name="relativePath">A relative path where the key file should be
        /// stored, generally specified as "/containerName/[subDir/]keys.xml".</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        /// <remarks>
        /// The container referenced by <paramref name="relativePath"/> must already exist.
        /// </remarks>
        public static IKeyRotationBuilder PersistKeysToAzureBlobStorage(this IKeyRotationBuilder builder, CloudStorageAccount storageAccount, string relativePath)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (storageAccount == null)
            {
                throw new ArgumentNullException(nameof(storageAccount));
            }
            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            // Simply concatenate the root storage endpoint with the relative path,
            // which includes the container name and blob name.

            var uriBuilder = new UriBuilder(storageAccount.BlobEndpoint);
            uriBuilder.Path = $"{uriBuilder.Path.TrimEnd('/')}/{relativePath.TrimStart('/')}";

            // We can create a CloudBlockBlob from the storage URI and the creds.

            var blobAbsoluteUri = uriBuilder.Uri;
            var credentials = storageAccount.Credentials;

            return PersistKeystoAzureBlobStorageInternal(builder, () => new CloudBlockBlob(blobAbsoluteUri, credentials));
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified path
        /// in Azure Blob Storage.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="blobUri">The full URI where the key file should be stored.
        /// The URI must contain the SAS token as a query string parameter.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        /// <remarks>
        /// The container referenced by <paramref name="blobUri"/> must already exist.
        /// </remarks>
        public static IKeyRotationBuilder PersistKeysToAzureBlobStorage(this IKeyRotationBuilder builder, Uri blobUri)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (blobUri == null)
            {
                throw new ArgumentNullException(nameof(blobUri));
            }

            var uriBuilder = new UriBuilder(blobUri);

            // The SAS token is present in the query string.

            if (string.IsNullOrEmpty(uriBuilder.Query))
            {
                throw new ArgumentException(
                    message: "URI does not have a SAS token in the query string.",
                    paramName: nameof(blobUri));
            }

            var credentials = new StorageCredentials(uriBuilder.Query);
            uriBuilder.Query = null; // no longer needed
            var blobAbsoluteUri = uriBuilder.Uri;

            return PersistKeystoAzureBlobStorageInternal(builder, () => new CloudBlockBlob(blobAbsoluteUri, credentials));
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified path
        /// in Azure Blob Storage.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="blobReference">The <see cref="CloudBlockBlob"/> where the
        /// key file should be stored.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        /// <remarks>
        /// The container referenced by <paramref name="blobReference"/> must already exist.
        /// </remarks>
        public static IKeyRotationBuilder PersistKeysToAzureBlobStorage(this IKeyRotationBuilder builder, CloudBlockBlob blobReference)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (blobReference == null)
            {
                throw new ArgumentNullException(nameof(blobReference));
            }

            // We're basically just going to make a copy of this blob.
            // Use (container, blobName) instead of (storageuri, creds) since the container
            // is tied to an existing service client, which contains user-settable defaults
            // like retry policy and secondary connection URIs.

            var container = blobReference.Container;
            var blobName = blobReference.Name;

            return PersistKeystoAzureBlobStorageInternal(builder, () => container.GetBlockBlobReference(blobName));
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified path
        /// in Azure Blob Storage.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="container">The <see cref="CloudBlobContainer"/> in which the
        /// key file should be stored.</param>
        /// <param name="blobName">The name of the key file, generally specified
        /// as "[subdir/]keys.xml"</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        /// <remarks>
        /// The container referenced by <paramref name="container"/> must already exist.
        /// </remarks>
        public static IKeyRotationBuilder PersistKeysToAzureBlobStorage(this IKeyRotationBuilder builder, CloudBlobContainer container, string blobName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (blobName == null)
            {
                throw new ArgumentNullException(nameof(blobName));
            }
            return PersistKeystoAzureBlobStorageInternal(builder, () => container.GetBlockBlobReference(blobName));
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

            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    options.XmlRepository = new EntityFrameworkCoreXmlRepository<TContext>(services, loggerFactory);
                });
            });
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

            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    options.XmlRepository = new FileSystemXmlRepository(directory, loggerFactory);
                });
            });
            return builder;
        }

        private const string DataProtectionKeysName = "DataProtection-Keys";

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
            return PersistKeysToStackExchangeRedis(builder, connectionMultiplexer, DataProtectionKeysName);
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

            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyManagementOptions>(options =>
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

            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
            {
                var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                var certificateResolver = services.GetRequiredService<AspNetCore.DataProtection.XmlEncryption.ICertificateResolver>();
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    options.XmlEncryptor = new CertificateXmlEncryptor(thumbprint, certificateResolver, loggerFactory);
                });
            });

            return builder;
        }

        // important: the Func passed into this method must return a new instance with each call
        private static IKeyRotationBuilder PersistKeystoAzureBlobStorageInternal(IKeyRotationBuilder builder, Func<CloudBlockBlob> blobRefFactory)
        {
            builder.Services.Configure<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new AzureBlobXmlRepository(blobRefFactory);
            });
            return builder;
        }

        private static IKeyRotationBuilder PersistKeysToStackExchangeRedisInternal(IKeyRotationBuilder builder, Func<IDatabase> databaseFactory, RedisKey key)
        {
            builder.Services.Configure<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new RedisXmlRepository(databaseFactory, key);
            });
            return builder;
        }
    }
}
