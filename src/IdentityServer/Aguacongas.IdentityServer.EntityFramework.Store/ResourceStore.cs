using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{

    /// <summary>
    /// <see cref="IResourceStore"/> implemtation
    /// </summary>
    /// <seealso cref="IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        private readonly ConfigurationDbContext _context;

        public ResourceStore(ConfigurationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var query = from api in _context.Apis
                            .Include(a => a.ApiClaims)
                            .Include(a => a.Secrets)
                            .Include(a => a.Resources)
                            .Include(a => a.Properties)
                            .Include(a => a.ApiScopes)
                        where apiResourceNames.Contains(api.Id)
                        select api;
            return await query
                .AsNoTracking()
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
            var query = from api in _context.Apis
                            .Include(a => a.ApiClaims)
                            .Include(a => a.Secrets)
                            .Include(a => a.Resources)
                            .Include(a => a.Properties)
                            .Include(a => a.ApiScopes)
                        where api.ApiScopes.Any(s => scopeNames.Contains(s.ApiScopeId))
                        select api;

            return await query
                .AsNoTracking()
                .Select(a => a.ToApi())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets API scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var query = from api in _context.ApiScopes
                            .Include(a => a.ApiScopeClaims)
                            .Include(a => a.Properties)
                            .Include(a => a.Resources)
                        join scope in _context.ApiScopes
                        on api.Id equals scope.Id
                        where scopeNames.Contains(scope.Id)
                        select api;

            return await query
                .AsNoTracking()
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
            var query = from identity in _context.Identities
                            .Include(i => i.IdentityClaims)
                            .Include(i => i.Properties)
                            .Include(i => i.Resources)
                        where scopeNames.Contains(identity.Id)
                        select identity;

            return await query
                .AsNoTracking()
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
                ApiResources = await _context.Apis
                    .Include(a => a.ApiClaims)
                    .Include(a => a.Secrets)
                    .Include(a => a.Properties)
                    .Include(a => a.Resources)
                    .Include(a => a.ApiScopes)
                    .AsNoTracking()
                    .Select(a => a.ToApi())
                    .ToListAsync()
                    .ConfigureAwait(false),
                IdentityResources = await _context.Identities
                    .Include(i => i.IdentityClaims)
                    .Include(i => i.Properties)
                    .Include(i => i.Resources)
                    .AsNoTracking()
                    .Select(i => i.ToIdentity())
                    .ToListAsync()
                    .ConfigureAwait(false),
                ApiScopes = await _context.ApiScopes
                    .Include(s => s.ApiScopeClaims)
                    .Include(s => s.Properties)
                    .Include(i => i.Resources)
                    .AsNoTracking()
                    .Select(s => s.ToApiScope())
                    .ToListAsync()
                    .ConfigureAwait(false)
            };
        }
    }
}
