// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;

namespace Aguacongas.IdentityServer.RavenDb.Store.Identity
{
    public abstract class IdentityClaimStore : IdentitySubEntityStoreBase<IdentityClaim>
    {
        protected IdentityClaimStore(IAsyncDocumentSession session, ILogger<AdminStore<IdentityClaim>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToIdentity(IdentityResource identity, string id)
        {
            identity.IdentityClaims.Add(new IdentityClaim
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromIdentity(IdentityResource identity, string id)
        {
            identity.IdentityClaims.Remove(identity.IdentityClaims.First(e => e.Id == id));
        }
    }
}
