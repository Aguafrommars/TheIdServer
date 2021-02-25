// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientUriStore : ClientSubEntityStoreBase<Entity.ClientUri>
    {
        public ClientUriStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientUri>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ClientUri> GetCollection(Entity.Client client)
        {
            if (client.RedirectUris == null)
            {
                client.RedirectUris = new List<Entity.ClientUri>();
            }

            return client.RedirectUris;
        }
    }
}
