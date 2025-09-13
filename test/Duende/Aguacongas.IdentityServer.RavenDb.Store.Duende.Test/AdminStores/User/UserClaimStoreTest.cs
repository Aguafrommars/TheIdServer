// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User;
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.User
{
    public class UserClaimStoreTest : UserSubEntityStoreTestBase<Entity.UserClaim>
    {
        protected override IAdminStore<Entity.UserClaim> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<Entity.UserClaim>> logger)
        => new UserClaimStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<Entity.UserClaim> GetCollection(Entity.User user)
        {
            if (user.UserClaims == null)
            {
                user.UserClaims = new List<Entity.UserClaim>();
            }

            return user.UserClaims;
        }
    }
}
