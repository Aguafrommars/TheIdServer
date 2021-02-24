// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityServer4.Services;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    /// <summary>
    /// <see cref="ICorsPolicyService"/> implementation
    /// </summary>
    /// <seealso cref="ICorsPolicyService" />
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IAsyncDocumentSession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsPolicyService"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public CorsPolicyService(IAsyncDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            var corsUri = new Uri(origin);
            var corsValue = Entity.UriKinds.Cors;
            var sanetized = $"{corsUri.Scheme.ToUpperInvariant()}://{corsUri.Host.ToUpperInvariant()}:{corsUri.Port}";
            return _session.Query<Entity.ClientUri>()
                .AnyAsync(u => (u.Kind & corsValue) == corsValue && u.SanetizedCorsUri == sanetized);
        }

    }
}
