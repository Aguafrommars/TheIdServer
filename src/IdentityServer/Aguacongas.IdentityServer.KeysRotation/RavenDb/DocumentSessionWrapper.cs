// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System;
using Raven.Client.Documents.Session;

namespace Aguacongas.IdentityServer.KeysRotation.RavenDb
{
    public class DocumentSessionWrapper
    {
        public IDocumentSession Session { get; }

        public DocumentSessionWrapper(IDocumentSession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }
    }
}
