// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System;

namespace Aguacongas.IdentityServer.MongoDb.Store.Test.AdminStores
{
    public class AdminStoreOfUserClaimTest : AdminStoreTestBase<UserClaim>
    {
        protected override object CreateParentEntiy(Type parentType)
        {
            return new User
            {
                UserName = Guid.NewGuid().ToString(),
                Email = $"{Guid.NewGuid()}@sample.com"
            };
        }
    }
}
