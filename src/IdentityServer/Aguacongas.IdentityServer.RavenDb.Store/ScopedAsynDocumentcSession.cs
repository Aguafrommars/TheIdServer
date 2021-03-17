// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Raven.Client.Documents.Session;
using System;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class ScopedAsynDocumentcSession
    {
        public IAsyncDocumentSession Session { get; }

        public ScopedAsynDocumentcSession(IAsyncDocumentSession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }
    }
}
