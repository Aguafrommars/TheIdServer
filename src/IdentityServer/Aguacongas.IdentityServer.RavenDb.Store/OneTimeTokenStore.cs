// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class OneTimeTokenStore : AdminStore<OneTimeToken>
    {
        public OneTimeTokenStore(IAsyncDocumentSession session, ILogger<AdminStore<OneTimeToken>> logger)
            : base(session, logger)
        {
        }
    }
}
