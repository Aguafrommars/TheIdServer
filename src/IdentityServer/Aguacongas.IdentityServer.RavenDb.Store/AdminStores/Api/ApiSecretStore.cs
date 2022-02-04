// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiSecretStore : ApiSubEntityStoreBase<ApiSecret>
    {
        public ApiSecretStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<ApiSecret>> logger) : base(session, logger)
        {
        }

        protected override ICollection<ApiSecret> GetCollection(ProtectResource api)
        {
            if (api.Secrets == null)
            {
                api.Secrets = new List<ApiSecret>();
            }

            return api.Secrets;
        }
    }
}
