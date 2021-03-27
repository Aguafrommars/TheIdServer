// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Raven.Client.Documents;
using Raven.TestDriver;
using System;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    class RavenDbTestDriverWrapper : RavenTestDriver
    {
        public new IDocumentStore GetDocumentStore(GetDocumentStoreOptions options = null, string database = null)
            => base.GetDocumentStore(options, database ?? Guid.NewGuid().ToString());

        protected override void PreInitialize(IDocumentStore documentStore)
        {
            documentStore.SetFindIdentityPropertyForIdentityServerStores();
            base.PreInitialize(documentStore);
        }
    }
}
