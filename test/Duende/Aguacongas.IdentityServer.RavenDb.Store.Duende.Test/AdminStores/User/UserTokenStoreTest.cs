// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User;
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.User
{
    public class UserTokenStoreTest : UserSubEntityStoreTestBase<Entity.UserToken>
    {
        protected override IAdminStore<Entity.UserToken> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<Entity.UserToken>> logger)
        => new UserTokenStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<Entity.UserToken> GetCollection(Entity.User user)
        {
            if (user.UserTokens == null)
            {
                user.UserTokens = new List<Entity.UserToken>();
            }

            return user.UserTokens;
        }
    }
}
