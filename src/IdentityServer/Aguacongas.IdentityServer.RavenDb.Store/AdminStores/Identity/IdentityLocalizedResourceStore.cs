// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;

namespace Aguacongas.IdentityServer.RavenDb.Store.Identity
{
    public abstract class IdentityLocalizedResourceStore : IdentitySubEntityStoreBase<IdentityLocalizedResource>
    {
        protected IdentityLocalizedResourceStore(IAsyncDocumentSession session, ILogger<AdminStore<IdentityLocalizedResource>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToIdentity(IdentityResource identity, string id)
        {
            identity.Resources.Add(new IdentityLocalizedResource
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromIdentity(IdentityResource identity, string id)
        {
            identity.Resources.Remove(identity.Resources.First(e => e.Id == id));
        }
    }
}
