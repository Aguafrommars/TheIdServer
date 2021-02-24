// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientSecretStore : ClientSubEntityStoreBase<Entity.ClientSecret>
    {
        public ClientSecretStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientSecret>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToClient(Entity.Client client, string id)
        {
            client.ClientSecrets.Add(new Entity.ClientSecret
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromClient(Entity.Client client, string id)
        {
            client.ClientSecrets.Remove(client.ClientSecrets.First(e => e.Id == id));
        }
    }
}
