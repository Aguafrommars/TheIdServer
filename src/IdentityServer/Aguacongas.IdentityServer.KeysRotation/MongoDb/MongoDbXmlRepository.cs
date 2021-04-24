// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Aguacongas.IdentityServer.KeysRotation.MongoDb
{
    public class MongoDbXmlRepository<TKey> : IXmlRepository
        where TKey: IXmlKey, new()
    {
        private readonly ILogger<MongoDbXmlRepository<TKey>> _logger;
        private readonly IServiceProvider _services;

        public MongoDbXmlRepository(IServiceProvider services, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<MongoDbXmlRepository<TKey>>();
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public virtual IReadOnlyCollection<XElement> GetAllElements()
        {
            using var scope = _services.CreateScope();
            var queryable = scope.ServiceProvider.GetRequiredService<MongoCollectionWrapper<TKey>>().Queryable;
            // Put logger in a local such that `this` isn't captured.
            var logger = _logger;
            var list = queryable.ToList();
            return list
                .Select(key => TryParseKeyXml(key.Xml, logger))
                .ToList()
                .AsReadOnly();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using var scope = _services.CreateScope();
            var collection = scope.ServiceProvider.GetRequiredService<MongoCollectionWrapper<TKey>>().Collection;

            var newKey = new TKey
            {
                Id = Guid.NewGuid().ToString(),
                FriendlyName = friendlyName,
                Xml = element.ToString(SaveOptions.DisableFormatting)
            };
            collection.InsertOne(newKey);
        }

        private static XElement TryParseKeyXml(string xml, ILogger logger)
        {
            try
            {
                return XElement.Parse(xml);
            }
            catch (Exception e)
            {
                logger?.LogWarning(e, "An exception occurred while parsing the key xml '{Xml}'.", xml);
                return null;
            }
        }
    }

}
