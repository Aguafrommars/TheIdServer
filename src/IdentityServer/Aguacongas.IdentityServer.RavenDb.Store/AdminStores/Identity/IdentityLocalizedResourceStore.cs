// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Identity
{
    public class IdentityLocalizedResourceStore : IdentitySubEntityStoreBase<IdentityLocalizedResource>
    {
        public IdentityLocalizedResourceStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<IdentityLocalizedResource>> logger) : base(session, logger)
        {
        }

        protected override ICollection<IdentityLocalizedResource> GetCollection(IdentityResource identity)
        {
            if (identity.Resources == null)
            {
                identity.Resources = new List<IdentityLocalizedResource>();
            }

            return identity.Resources;
        }
    }
}
