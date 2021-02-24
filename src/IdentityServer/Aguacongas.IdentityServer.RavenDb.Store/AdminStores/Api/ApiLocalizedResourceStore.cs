// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiLocalizedResourceStore : ApiSubEntityStoreBase<ApiLocalizedResource>
    {
        public ApiLocalizedResourceStore(IAsyncDocumentSession session, ILogger<AdminStore<ApiLocalizedResource>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToApi(ProtectResource api, string id)
        {
            api.Resources.Add(new ApiLocalizedResource
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromApi(ProtectResource api, string id)
        {
            api.Resources.Remove(api.Resources.First(e => e.Id == id));
        }
    }
}
