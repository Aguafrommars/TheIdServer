// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataProtectionBuilderExtensions
    {
        public static IDataProtectionBuilder PersistKeysToRavenDb<TWrapper>(this IDataProtectionBuilder builder, Func<IServiceProvider, IDocumentSession> getSession = null)
            where TWrapper: DocumentSessionWrapper
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
                        options.XmlRepository = new RavenDbXmlRepository<DataProtectionKey, TWrapper>(services, loggerFactory);
                    });
                })
                .AddTransient(p => new DocumentSessionWrapper(getSession(p)));

            return builder;
        }
    }
}
