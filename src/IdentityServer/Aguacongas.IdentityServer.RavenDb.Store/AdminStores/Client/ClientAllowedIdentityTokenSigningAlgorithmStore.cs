// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientAllowedIdentityTokenSigningAlgorithmStore : ClientSubEntityStoreBase<Entity.ClientAllowedIdentityTokenSigningAlgorithm>
    {
        public ClientAllowedIdentityTokenSigningAlgorithmStore(ScopedAsynDocumentcSession session, 
            ILogger<AdminStore<Entity.ClientAllowedIdentityTokenSigningAlgorithm>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ClientAllowedIdentityTokenSigningAlgorithm> GetCollection(Entity.Client client)
        {
            if (client.AllowedIdentityTokenSigningAlgorithms == null)
            {
                client.AllowedIdentityTokenSigningAlgorithms = new List<Entity.ClientAllowedIdentityTokenSigningAlgorithm>();
            }

            return client.AllowedIdentityTokenSigningAlgorithms;
        }
    }
}
