// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiLocalizedResourceStore : ApiSubEntityStoreBase<ApiLocalizedResource>
    {
        public ApiLocalizedResourceStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<ApiLocalizedResource>> logger) : base(session, logger)
        {
        }

        protected override ICollection<ApiLocalizedResource> GetCollection(ProtectResource api)
        {
            if (api.Resources == null)
            {
                api.Resources = new List<ApiLocalizedResource>();
            }

            return api.Resources;
        }
    }
}
