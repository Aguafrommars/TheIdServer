// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiSecretStore : ApiSubEntityStoreBase<ApiSecret>
    {
        public ApiSecretStore(IAsyncDocumentSession session, ILogger<AdminStore<ApiSecret>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToApi(ProtectResource api, string id)
        {
            api.Secrets.Add(new ApiSecret
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromApi(ProtectResource api, string id)
        {
            api.Secrets.Remove(api.Secrets.First(e => e.Id == id));
        }
    }
}
