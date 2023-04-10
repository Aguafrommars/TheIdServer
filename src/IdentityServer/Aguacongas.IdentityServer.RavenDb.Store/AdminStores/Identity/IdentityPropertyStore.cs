// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Identity
{
    public class IdentityPropertyStore : IdentitySubEntityStoreBase<IdentityProperty>
    {
        public IdentityPropertyStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<IdentityProperty>> logger) : base(session, logger)
        {
        }

        protected override ICollection<IdentityProperty> GetCollection(IdentityResource identity)
        {
            if (identity.Properties == null)
            {
                identity.Properties = new List<IdentityProperty>();
            }

            return identity.Properties;
        }
    }
}
