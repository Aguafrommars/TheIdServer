// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User;
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.User
{
    public class UserRoleStoreTest : UserSubEntityStoreTestBase<Entity.UserRole>
    {
        protected override IAdminStore<Entity.UserRole> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<Entity.UserRole>> logger)
        => new UserRoleStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<Entity.UserRole> GetCollection(Entity.User user)
        {
            if (user.UserRoles == null)
            {
                user.UserRoles = new List<Entity.UserRole>();
            }

            return user.UserRoles;
        }
    }
}
