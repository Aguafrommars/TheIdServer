// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User
{
    public class UserClaimStore : UserSubEntityStoreBase<Entity.UserClaim>
    {
        public UserClaimStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.UserClaim>> logger) : base(session, logger)
        {
        }

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
