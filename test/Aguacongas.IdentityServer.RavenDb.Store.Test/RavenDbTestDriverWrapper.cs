// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Raven.Client.Documents;
using Raven.TestDriver;
using System.Runtime.CompilerServices;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test
{
    class RavenDbTestDriverWrapper : RavenTestDriver
    {
        public new IDocumentStore GetDocumentStore(GetDocumentStoreOptions options = null, [CallerMemberName] string database = null)
            => base.GetDocumentStore(options, database);

        protected override void PreInitialize(IDocumentStore documentStore)
        {
            documentStore.SetFindIdentityPropertyForIdentityModel();
            var func = documentStore.Conventions.FindIdentityProperty;
            documentStore.Conventions.FindIdentityProperty = memberInfo =>
            {
                if (memberInfo.DeclaringType.Assembly == typeof(ProtectResource).Assembly)
                {
                    return false;
                }
                return func(memberInfo);
            };
            base.PreInitialize(documentStore);
        }
    }
}
