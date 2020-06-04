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
        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var query = from api in _context.Apis
                            .Include(a => a.ApiClaims)
                            .Include(a => a.ApiScopeClaims)
                            .Include(a => a.Secrets)
                            .Include(a => a.Resources)
                            .Include(a => a.Properties)
                            .Include(a => a.Scopes)
                            .ThenInclude(s => s.Resources)
                            .Include(a => a.Scopes)
                            .ThenInclude(s => s.ApiScopeClaims)
                        select api;
            var entity = await query.FirstOrDefaultAsync(api => api.Id == name).ConfigureAwait(false);

            return entity.ToApi();
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var query = from api in _context.Apis
                            .Include(a => a.ApiClaims)
                            .Include(a => a.ApiScopeClaims)
                            .Include(a => a.Secrets)
                            .Include(a => a.Resources)
                            .Include(a => a.Properties)
                            .Include(a => a.Scopes)
                            .ThenInclude(s => s.Resources)
                            .Include(a => a.Scopes)
                            .ThenInclude(s => s.ApiScopeClaims)
                        select api;

            return await query
                .AsNoTracking()
                .Select(a => a.ToApi())
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
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
                    .Include(a => a.ApiScopeClaims)
                    .Include(a => a.Secrets)
                    .Include(a => a.Properties)
                    .Include(a => a.Resources)
                    .Include(a => a.Scopes)
                    .ThenInclude(s => s.ApiScopeClaims)
                    .Include(a => a.Scopes)
                    .ThenInclude(s => s.Resources)
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
                    .ConfigureAwait(false)
            };
        }
    }
}
