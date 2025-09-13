// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Raven.Client.Documents;
using Raven.TestDriver;
using System;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test
{
    class RavenDbTestDriverWrapper : RavenTestDriver
    {
        static RavenDbTestDriverWrapper() 
        { 
            ConfigureServer(new TestServerOptions
            {
                Licensing = new Raven.Embedded.ServerOptions.LicensingOptions
                {
                    ThrowOnInvalidOrMissingLicense = false
                }
            });
        }
        public new IDocumentStore GetDocumentStore(GetDocumentStoreOptions options = null, string database = null)
            => base.GetDocumentStore(options, database ?? Guid.NewGuid().ToString());

        protected override void PreInitialize(IDocumentStore documentStore)
        {
            documentStore.SetFindIdentityPropertyForIdentityServerStores();
            var findId = documentStore.Conventions.FindIdentityProperty;
            documentStore.Conventions.FindIdentityProperty = memberInfo =>
            {
                if (memberInfo.DeclaringType == typeof(Store.AdminStores.Test.AdminStoreTest.TestEntity))
                {
                    return false;
                }
                return findId(memberInfo);
            };
            base.PreInitialize(documentStore);
        }
    }
}
