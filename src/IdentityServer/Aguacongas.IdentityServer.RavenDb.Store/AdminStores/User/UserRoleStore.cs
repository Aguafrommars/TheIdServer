// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User
{
    public class UserRoleStore : UserSubEntityStoreBase<Entity.UserRole>
    {
        public UserRoleStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.UserRole>> logger) : base(session, logger)
        {
        }

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
