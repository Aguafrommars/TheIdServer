// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models = IdentityServer4.Models;

namespace Aguacongas.IdentityServer.Store
{

    /// <summary>
    /// <see cref="IResourceStore"/> implemtation
    /// </summary>
    /// <seealso cref="IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        private readonly IAdminStore<ProtectResource> _apiStore;
        private readonly IAdminStore<IdentityResource> _identityStore;
        private readonly IAdminStore<ApiScope> _apiScopeStore;
        private readonly IAdminStore<ApiApiScope> _apiApiScopeStore;

        public ResourceStore(IAdminStore<ProtectResource> apiStore,
            IAdminStore<IdentityResource> identityStore,
            IAdminStore<ApiScope> apiScopeStore,
            IAdminStore<ApiApiScope> apiApiScopeStore)
        {
            _apiStore = apiStore ?? throw new ArgumentNullException(nameof(apiStore));
            _identityStore = identityStore ?? throw new ArgumentNullException(nameof(identityStore));
            _apiScopeStore = apiScopeStore ?? throw new ArgumentNullException(nameof(apiScopeStore));
            _apiApiScopeStore = apiApiScopeStore ?? throw new ArgumentNullException(nameof(apiApiScopeStore));
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        /// <param name="apiResourceNames">The name.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var list = new List<PageResponse<ProtectResource>>(apiResourceNames.Count());
            foreach (var name in apiResourceNames)
            {
                list.Add(await _apiStore.GetAsync(new PageRequest
                {
                    Take = null,
                    Filter = $"{nameof(ProtectResource.Id)} eq '{name}'",
                    Expand = $"{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}"
                }).ConfigureAwait(false));
            }
            return list
                .SelectMany(r => r.Items.Select(a => a.ToApi()));
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = new List<PageResponse<ApiApiScope>>(scopeNames.Count());
            foreach(var name in scopeNames)
            {
                list.Add(await _apiApiScopeStore.GetAsync(new PageRequest
                {
                    Take = null,
                    Select = nameof(ApiApiScope.ApiId),
                    Filter = $"{nameof(ApiApiScope.ApiScopeId)} eq '{name}'"
                }).ConfigureAwait(false));
            }
            
            var apiIdList = list.SelectMany(r => r.Items.Select(a => a.ApiId));

            var apiList = new List<ProtectResource>(apiIdList.Count());
            foreach (var id in apiIdList)
            {
                apiList.Add(await _apiStore.GetAsync(id, new GetRequest
                {
                    Expand = $"{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}"
                }).ConfigureAwait(false));
            }
            
            return apiList
                .Select(r => r.ToApi());
        }

        /// <summary>
        /// Gets API scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var list = new List<PageResponse<ApiScope>>(scopeNames.Count());
            foreach (var name in scopeNames)
            {
                list.Add(await _apiScopeStore.GetAsync(new PageRequest
                {
                    Take = null,
                    Filter = $"{nameof(ApiScope.Id)} eq '{name}'",
                    Expand = $"{nameof(ApiScope.ApiScopeClaims)},{nameof(ApiScope.Properties)},{nameof(ApiScope.Resources)}"
                }).ConfigureAwait(false));
            }
            return list.SelectMany(r => r.Items.Select(s => s.ToApiScope()));
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = new List<PageResponse<IdentityResource>>(scopeNames.Count());
            foreach (var name in scopeNames)
            {
                list.Add(await _identityStore.GetAsync(new PageRequest
                {
                    Take = null,
                    Filter = $"{nameof(IdentityResource.Id)} eq '{name}'",
                    Expand = $"{nameof(IdentityResource.IdentityClaims)},{nameof(IdentityResource.Properties)},{nameof(IdentityResource.Resources)}"
                }).ConfigureAwait(false));
            }

            return list
                .SelectMany(r => r.Items.Select(e => e.ToIdentity()));
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
                    Take = null,
                    Expand = $"{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}"
                }).ConfigureAwait(false)).Items.Select(a => a.ToApi()).ToList(),
                IdentityResources = (await _identityStore.GetAsync(new PageRequest
                {
                    Take = null,
                    Expand = $"{nameof(IdentityResource.IdentityClaims)},{nameof(IdentityResource.Properties)},{nameof(IdentityResource.Resources)}"
                }).ConfigureAwait(false)).Items.Select(i => i.ToIdentity()).ToList(),
                ApiScopes = (await _apiScopeStore.GetAsync(new PageRequest
                {
                    Take = null,
                    Expand = $"{nameof(ApiScope.ApiScopeClaims)},{nameof(ApiScope.Properties)},{nameof(ApiScope.Resources)}"
                }).ConfigureAwait(false)).Items.Select(s => s.ToApiScope()).ToList()
            };
        }
    }
}
