// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientIdpRestrictionStore : ClientSubEntityStoreBase<Entity.ClientIdpRestriction>
    {
        public ClientIdpRestrictionStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientIdpRestriction>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToClient(Entity.Client client, string id)
        {
            client.IdentityProviderRestrictions.Add(new Entity.ClientIdpRestriction
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromClient(Entity.Client client, string id)
        {
            client.IdentityProviderRestrictions.Remove(client.IdentityProviderRestrictions.First(e => e.Id == id));
        }
    }
}
