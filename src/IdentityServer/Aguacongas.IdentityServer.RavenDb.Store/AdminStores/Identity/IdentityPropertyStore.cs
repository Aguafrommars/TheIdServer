// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;

namespace Aguacongas.IdentityServer.RavenDb.Store.Identity
{
    public abstract class IdentityPropertyStore : IdentitySubEntityStoreBase<IdentityProperty>
    {
        protected IdentityPropertyStore(IAsyncDocumentSession session, ILogger<AdminStore<IdentityProperty>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToIdentity(IdentityResource identity, string id)
        {
            identity.Properties.Add(new IdentityProperty
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromIdentity(IdentityResource identity, string id)
        {
            identity.Properties.Remove(identity.Properties.First(e => e.Id == id));
        }
    }
}
