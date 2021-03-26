// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aguacongas.IdentityServer.KeysRotation.RavenDb
{
    public class RavenDbXmlRepository : IXmlRepository
    {
        private readonly ILogger<RavenDbXmlRepository> _logger;
        private readonly IAsyncDocumentSession _session;

        public RavenDbXmlRepository(IAsyncDocumentSession session, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<RavenDbXmlRepository>();
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            throw new NotImplementedException();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            throw new NotImplementedException();
        }
    }
}
