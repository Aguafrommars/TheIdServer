// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiLocalizedResourceStore : ApiSubEntityStoreBase<ApiLocalizedResource>
    {
        public ApiLocalizedResourceStore(IAsyncDocumentSession session, ILogger<AdminStore<ApiLocalizedResource>> logger) : base(session, logger)
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
