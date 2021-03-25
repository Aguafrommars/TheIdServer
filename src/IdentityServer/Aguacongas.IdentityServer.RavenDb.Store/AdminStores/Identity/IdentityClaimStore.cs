// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Identity
{
    public class IdentityClaimStore : IdentitySubEntityStoreBase<IdentityClaim>
    {
        public IdentityClaimStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<IdentityClaim>> logger) : base(session, logger)
        {
        }

        protected override ICollection<IdentityClaim> GetCollection(IdentityResource identity)
        {
            if (identity.IdentityClaims == null)
            {
                identity.IdentityClaims = new List<IdentityClaim>();
            }

            return identity.IdentityClaims;
        }
    }
}
