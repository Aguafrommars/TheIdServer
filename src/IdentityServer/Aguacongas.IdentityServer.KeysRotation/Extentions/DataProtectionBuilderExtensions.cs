// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using mongoDb = Aguacongas.IdentityServer.KeysRotation.MongoDb;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataProtectionBuilderExtensions
    {
        public static IDataProtectionBuilder PersistKeysToRavenDb(this IDataProtectionBuilder builder, Func<IServiceProvider, IDocumentSession> getSession = null)
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

            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
                {
                    var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                    return new ConfigureOptions<KeyManagementOptions>(options =>
                    {
                        options.XmlRepository = new RavenDbXmlRepository<DataProtectionKey>(services, loggerFactory);
                    });
                })
                .AddTransient(p => new DocumentSessionWrapper(getSession(p)));

            return builder;
        }

        public static IDataProtectionBuilder PersistKeysToMongoDb(this IDataProtectionBuilder builder, Func<IServiceProvider, IMongoCollection<mongoDb.DataProtectionKey>> getCollection = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (getCollection == null)
            {
                getCollection = p => p.GetRequiredService<IMongoDatabase>().GetCollection<mongoDb.DataProtectionKey>(nameof(mongoDb.DataProtectionKey));
            }

            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
                {
                    var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                    return new ConfigureOptions<KeyManagementOptions>(options =>
                    {
                        options.XmlRepository = new mongoDb.MongoDbXmlRepository<mongoDb.DataProtectionKey>(services, loggerFactory);
                    });
                })
                .AddTransient(p => new mongoDb.MongoCollectionWrapper<mongoDb.DataProtectionKey>(getCollection(p)));

            return builder;
        }
    }
}
