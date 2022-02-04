// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User
{
    public class UserLoginStore : UserSubEntityStoreBase<Entity.UserLogin>
    {
        public UserLoginStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.UserLogin>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.UserLogin> GetCollection(Entity.User user)
        {
            if (user.UserLogins == null)
            {
                user.UserLogins = new List<Entity.UserLogin>();
            }

            return user.UserLogins;
        }
    }
}
