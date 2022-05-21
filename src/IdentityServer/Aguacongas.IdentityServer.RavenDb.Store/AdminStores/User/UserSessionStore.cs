// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User
{
    public class UserSessionStore : UserSubEntityStoreBase<Entity.UserSession>
    {
        public UserSessionStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.UserSession>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.UserSession> GetCollection(Entity.User user)
        {
            if (user.UserSessions == null)
            {
                user.UserSessions = new List<Entity.UserSession>();
            }

            return user.UserSessions;
        }
    }
}
