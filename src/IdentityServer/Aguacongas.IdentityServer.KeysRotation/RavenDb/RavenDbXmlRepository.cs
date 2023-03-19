// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Aguacongas.IdentityServer.KeysRotation.RavenDb
{
    public class RavenDbXmlRepository<TKey> : IXmlRepository
        where TKey: IXmlKey, new()
    {
        private readonly ILogger<RavenDbXmlRepository<TKey>> _logger;
        private readonly IServiceProvider _services;

        public RavenDbXmlRepository(IServiceProvider services, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<RavenDbXmlRepository<TKey>>();
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public virtual IReadOnlyCollection<XElement> GetAllElements()
        {
            var session = _services.GetRequiredService<DocumentSessionWrapper>().Session;
            // Put logger in a local such that `this` isn't captured.
            var logger = _logger;
            return session.Query<TKey>()
                .AsEnumerable()
                .Select(key => TryParseKeyXml(key.Xml, logger))
                .ToList()
                .AsReadOnly();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var session = _services.GetRequiredService<DocumentSessionWrapper>().Session;

            var newKey = new TKey
            {
                FriendlyName = friendlyName,
                Xml = element.ToString(SaveOptions.DisableFormatting)
            };
            session.Store(newKey);
            session.SaveChanges();
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
