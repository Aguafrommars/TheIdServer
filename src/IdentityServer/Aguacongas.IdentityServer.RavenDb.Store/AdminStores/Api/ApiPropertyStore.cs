// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiPropertyStore : ApiSubEntityStoreBase<ApiProperty>
    {
        public ApiPropertyStore(IAsyncDocumentSession session, ILogger<AdminStore<ApiProperty>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToApi(ProtectResource api, string id)
        {
            api.Properties.Add(new ApiProperty
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromApi(ProtectResource api, string id)
        {
            api.Properties.Remove(api.Properties.First(e => e.Id == id));
        }
    }
}
