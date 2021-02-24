// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store
{

    /// <summary>
    /// <see cref="IResourceStore"/> implemtation
    /// </summary>
    /// <seealso cref="IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        private readonly IAsyncDocumentSession _session;

        public ResourceStore(IAsyncDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var query = from api in _session.Query<Entity.ProtectResource>()
                        where apiResourceNames.Contains(api.Id)
                        select api;
            return await query
                .Select(a => a.ToApi())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var query = from api in _session.Query<Entity.ProtectResource>()
                        where api.ApiScopes.Any(s => scopeNames.Contains(s.ApiScopeId))
                        select api;

            return await query
                .Select(a => a.ToApi())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets API scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IdentityServer4.Models.ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var query = from scope in _session.Query<Entity.ApiScope>()
                        where scopeNames.Contains(scope.Id)
                        select scope;

            return await query
                .Select(s => s.ToApiScope())
                .ToListAsync()
                .ConfigureAwait(false);
        }


        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var query = from identity in _session.Query<Entity.IdentityResource>()
                        where scopeNames.Contains(identity.Id)
                        select identity;

            return await query
                .Select(i => i.ToIdentity())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public async Task<Resources> GetAllResourcesAsync()
        {
            return new Resources
            {
                ApiResources = await _session.Query<Entity.ProtectResource>()
                    .Select(a => a.ToApi())
                    .ToListAsync()
                    .ConfigureAwait(false),
                IdentityResources = await _session.Query<Entity.IdentityResource>()
                    .Include(i => i.IdentityClaims)
                    .Include(i => i.Properties)
                    .Include(i => i.Resources)
                    .Select(i => i.ToIdentity())
                    .ToListAsync()
                    .ConfigureAwait(false),
                ApiScopes = await _session.Query<Entity.ApiScope>()
                    .Include(s => s.ApiScopeClaims)
                    .Include(s => s.Properties)
                    .Include(i => i.Resources)
                    .Select(s => s.ToApiScope())
                    .ToListAsync()
                    .ConfigureAwait(false)
            };
        }
    }
}
