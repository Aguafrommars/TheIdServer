// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Raven.Client.Documents.Session;
using System;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class ScopedAsynDocumentcSession : IDisposable
    {
        private bool disposedValue;

        public IAsyncDocumentSession Session { get; }

        public ScopedAsynDocumentcSession(IAsyncDocumentSession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Session.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
