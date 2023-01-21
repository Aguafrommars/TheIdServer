// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User
{
    public class UserTokenStore : UserSubEntityStoreBase<Entity.UserToken>
    {
        public UserTokenStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.UserToken>> logger) : base(session, logger)
        {
        }

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
