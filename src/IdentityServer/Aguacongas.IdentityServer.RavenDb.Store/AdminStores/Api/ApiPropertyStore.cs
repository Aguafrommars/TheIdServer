// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiPropertyStore : ApiSubEntityStoreBase<ApiProperty>
    {
        public ApiPropertyStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<ApiProperty>> logger) : base(session, logger)
        {
        }

        protected override ICollection<ApiProperty> GetCollection(ProtectResource api)
        {
            if (api.Properties == null)
            {
                api.Properties = new List<ApiProperty>();
            }

            return api.Properties;
        }
    }
}
