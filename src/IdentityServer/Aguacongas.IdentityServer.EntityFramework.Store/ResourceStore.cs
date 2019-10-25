using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{

    /// <summary>
    /// <see cref="IResourceStore"/> implemtation
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        private readonly IdentityServerDbContext _context;

        public ResourceStore(IdentityServerDbContext context)
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
            var entity = await _context.Apis
                .Include(a => a.ApiClaims)
                .Include(a => a.ApiScopeClaims)
                .Include(a => a.Secrets)
                .Include(a => a.Scopes)
                .Include(a => a.Properties)
                .FirstOrDefaultAsync(a => a.Id == name).ConfigureAwait(false);
            return entity.ToApi();
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var query = from scope in _context.ApiScopes
                        join api in _context.Apis
                        on scope.ApiId equals api.Id
                        where scopeNames.Contains(scope.Scope)
                        select api;

            return await query
                .Include(a => a.Scopes)
                .Include(a => a.ApiClaims)
                .Include(a => a.ApiScopeClaims)
                .Include(a => a.Secrets)
                .Include(a => a.Properties)
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
            return await _context.Identities
                .Include(i => i.IdentityClaims)
                .Include(i => i.Properties)
                .Where(i => scopeNames.Contains(i.Id))
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
                    .Include(a => a.Scopes)
                    .Include(a => a.Properties)
                    .Select(a => a.ToApi()).ToListAsync().ConfigureAwait(false),
                IdentityResources = await _context.Identities
                    .Include(i => i.IdentityClaims)
                    .Include(i => i.Properties)
                    .Select(i => i.ToIdentity()).ToListAsync().ConfigureAwait(false)
            };
        }
    }
}
