using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models = IdentityServer4.Models;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{

    /// <summary>
    /// <see cref="IResourceStore"/> implemtation
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        private readonly IAdminStore<ProtectResource> _apiStore;
        private readonly IAdminStore<IdentityResource> _identityStore;

        public ResourceStore(IAdminStore<ProtectResource> apiStore, IAdminStore<IdentityResource> identityStore)
        {
            _apiStore = apiStore ?? throw new ArgumentNullException(nameof(apiStore));
            _identityStore = identityStore ?? throw new ArgumentNullException(nameof(identityStore));
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task<Models.ApiResource> FindApiResourceAsync(string name)
        {
            var entity = await _apiStore.GetAsync(name, new GetRequest
            {
                Expand = "ApiClaims,ApiScopeClaims,Secrets,Scopes,Properties"
            }).ConfigureAwait(false);
            return entity.ToApi();
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var taskList = new List<Task<PageResponse<ProtectResource>>>(scopeNames.Count());
            foreach(var name in scopeNames)
            {
                taskList.Add(_apiStore.GetAsync(new PageRequest
                {
                    Filter = $"Scopes.Scope eq '{name}'",
                    Expand = "ApiClaims,ApiScopeClaims,Secrets,Scopes,Properties"
                }));
            }
            await Task.WhenAll(taskList)
                .ConfigureAwait(false);

            return taskList
                .SelectMany(t => t.Result.Items.Select(a => a.ToApi()));
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var taskList = new List<Task<IdentityResource>>(scopeNames.Count());
            foreach (var name in scopeNames)
            {
                taskList.Add(_identityStore.GetAsync(name, new GetRequest
                {
                    Expand = "IdentityClaims,Properties"
                }));
            }
            await Task.WhenAll(taskList)
                .ConfigureAwait(false);

            return taskList
                .Select(t => t.Result.ToIdentity());
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public async Task<Models.Resources> GetAllResourcesAsync()
        {
            return new Models.Resources
            {
                ApiResources = (await _apiStore.GetAsync(new PageRequest
                {
                    Expand = "ApiClaims,ApiScopeClaims,Secrets,Scopes,Properties"
                }).ConfigureAwait(false)).Items.Select(a => a.ToApi()).ToList(),
                IdentityResources = (await _identityStore.GetAsync(new PageRequest
                {
                    Expand = "IdentityClaims,Properties"
                }).ConfigureAwait(false)).Items.Select(i => i.ToIdentity()).ToList()
            };
        }
    }
}
