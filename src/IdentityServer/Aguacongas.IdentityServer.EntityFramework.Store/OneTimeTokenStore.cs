// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class OneTimeTokenStore : AdminStore<OneTimeToken, OperationalDbContext>
    {
        public OneTimeTokenStore(OperationalDbContext context, ILogger<AdminStore<OneTimeToken, OperationalDbContext>> logger)
            : base(context, logger)
        {
        }
    }
}
