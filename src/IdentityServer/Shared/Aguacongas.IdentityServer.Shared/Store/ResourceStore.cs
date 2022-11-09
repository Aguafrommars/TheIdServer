// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
#if DUENDE
using Duende.IdentityServer.Stores;
using IsModels = Duende.IdentityServer.Models;
#else
using IdentityServer4.Stores;
using IsModels = IdentityServer4.Models;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<IsModels.ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var filter = string.Join(" or ", apiResourceNames.Select(s => $"{nameof(ProtectResource.Id)} eq '{s}'"));
            var response = await _apiStore.GetAsync(new PageRequest
            {
                Filter = filter,
                Expand = $"{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}"
            }).ConfigureAwait(false);

            return response.Items.Select(a => a.ToApi());
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IsModels.ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var filter = string.Join(" or ", scopeNames.Select(s => $"{nameof(ApiApiScope.ApiScopeId)} eq '{s}'"));
            var apiIdListResponse = await _apiApiScopeStore.GetAsync(new PageRequest
            {
                Select = nameof(ApiApiScope.ApiId),
                Filter = filter
            }).ConfigureAwait(false);

            if (!apiIdListResponse.Items.Any())
            {
                return Array.Empty<IsModels.ApiResource>();
            }

            filter = string.Join(" or ", apiIdListResponse.Items.Select(i => $"{nameof(ProtectResource.Id)} eq '{i.ApiId}'"));
            var apiResposne = await _apiStore.GetAsync(new PageRequest
            {
                Filter = filter,
                Expand = $"{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}"
            }).ConfigureAwait(false);
            
            return apiResposne.Items.Select(r => r.ToApi());
        }

        /// <summary>
        /// Gets API scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IsModels.ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var filter = string.Join(" or ", scopeNames.Select(s => $"{nameof(ApiScope.Id)} eq '{s}'"));
            var response = await _apiScopeStore.GetAsync(new PageRequest
            {
                Filter = filter,
                Expand = $"{nameof(ApiScope.ApiScopeClaims)},{nameof(ApiScope.Properties)},{nameof(ApiScope.Resources)}"
            }).ConfigureAwait(false);
            return response.Items.Select(s => s.ToApiScope());
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IsModels.IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var filter = string.Join(" or ", scopeNames.Select(s => $"{nameof(IdentityResource.Id)} eq '{s}'"));
            var response = await _identityStore.GetAsync(new PageRequest
            {
                Filter = filter,
                Expand = $"{nameof(IdentityResource.IdentityClaims)},{nameof(IdentityResource.Properties)},{nameof(IdentityResource.Resources)}"
            }).ConfigureAwait(false);

            return response.Items.Select(e => e.ToIdentity());
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public async Task<IsModels.Resources> GetAllResourcesAsync()
        {
            return new IsModels.Resources
            {
                ApiResources = (await _apiStore.GetAsync(new PageRequest
                {
                    Expand = $"{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}"
                }).ConfigureAwait(false)).Items.Select(a => a.ToApi()).ToList(),
                IdentityResources = (await _identityStore.GetAsync(new PageRequest
                {
                    Expand = $"{nameof(IdentityResource.IdentityClaims)},{nameof(IdentityResource.Properties)},{nameof(IdentityResource.Resources)}"
                }).ConfigureAwait(false)).Items.Select(i => i.ToIdentity()).ToList(),
                ApiScopes = (await _apiScopeStore.GetAsync(new PageRequest
                {
                    Expand = $"{nameof(ApiScope.ApiScopeClaims)},{nameof(ApiScope.Properties)},{nameof(ApiScope.Resources)}"
                }).ConfigureAwait(false)).Items.Select(s => s.ToApiScope()).ToList()
            };
        }
    }
}
