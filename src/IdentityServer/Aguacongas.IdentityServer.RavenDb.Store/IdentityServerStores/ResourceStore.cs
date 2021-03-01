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
        private static readonly string[] _expandApiProperttyList = new[]
        {
            nameof(Entity.ProtectResource.ApiClaims),
            nameof(Entity.ProtectResource.Secrets),
            nameof(Entity.ProtectResource.ApiScopes),
            nameof(Entity.ProtectResource.Properties),
            nameof(Entity.ProtectResource.Resources)
        };
        private static readonly string[] _expandApiScopeProperttyList = new[]
        {
            nameof(Entity.ApiScope.ApiScopeClaims),
            nameof(Entity.ApiScope.Properties),
            nameof(Entity.ApiScope.Resources)
        };
        private static readonly string[] _expandIdentityProperttyList = new[]
        {
            nameof(Entity.IdentityResource.IdentityClaims),
            nameof(Entity.IdentityResource.Properties),
            nameof(Entity.IdentityResource.Resources)
        };

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
            var apiDictionary = await _session.LoadAsync<Entity.ProtectResource>(apiResourceNames.Select(n => $"{nameof(Entity.ProtectResource).ToLowerInvariant()}/{n}"),
                builder => builder.Expand(string.Join(",", _expandApiProperttyList))).ConfigureAwait(false);

            return await ToApiResourceList(apiDictionary.Values.Where(v => v != null)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var apiScopeDictionary = await _session.LoadAsync<Entity.ApiScope>(scopeNames.Select(n => $"{nameof(Entity.ApiScope).ToLowerInvariant()}/{n}"),
                    builder => builder.Expand(nameof(Entity.ApiScope.Apis))).ConfigureAwait(false);
            var dictionary = new Dictionary<string, Entity.ProtectResource>();
            foreach (var apiScope in apiScopeDictionary.Values.Where(v => v!= null))
            {
                var apiApiScopeList = await _session.LoadAsync<Entity.ApiApiScope>(apiScope.Apis.Select(a => a.Id)).ConfigureAwait(false);
                var apiList = await _session.LoadAsync<Entity.ProtectResource>(apiApiScopeList.Select(s => $"{nameof(Entity.ProtectResource).ToLowerInvariant()}/{s.Value.ApiId}"),
                        builder => builder.Expand(string.Join(",", _expandApiProperttyList))).ConfigureAwait(false);
                foreach(var api in apiList.Select(a => a.Value))
                {
                    dictionary[api.Id] = api;
                }
            }

            return await ToApiResourceList(dictionary.Values).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets API scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IdentityServer4.Models.ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var apiScopeDictionary = await _session.LoadAsync<Entity.ApiScope>(scopeNames.Select(n => $"{nameof(Entity.ApiScope).ToLowerInvariant()}/{n}"),
                    builder => builder.Expand(string.Join(",", _expandApiScopeProperttyList))).ConfigureAwait(false); 
            
            return await ToApiScopeList(apiScopeDictionary.Values.Where(v => v != null)).ConfigureAwait(false);
        }


        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var identiyDictionary = await _session.LoadAsync<Entity.IdentityResource>(scopeNames.Select(n => $"{nameof(Entity.IdentityResource).ToLowerInvariant()}/{n}"),
                    builder => builder.Expand(string.Join(",", _expandIdentityProperttyList))).ConfigureAwait(false);
            
            return await ToIdentityResourceList(identiyDictionary.Values.Where(v => v != null)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiQuery = from api in _session.Query<Entity.ProtectResource>()
                                .Expand(string.Join(",", _expandApiProperttyList))
                           select api;

            var apiScopeQuery = from scope in _session.Query<Entity.ApiScope>()
                            .Expand(string.Join(",", _expandApiScopeProperttyList))
                        select scope;

            var identityQuery = from identity in _session.Query<Entity.IdentityResource>()
                            .Expand(string.Join(",", _expandIdentityProperttyList))
                        select identity;

            var apiList = await apiQuery.ToListAsync().ConfigureAwait(false);
            var apiScopeList = await apiScopeQuery.ToListAsync().ConfigureAwait(false);
            var identityList = await identityQuery.ToListAsync().ConfigureAwait(false);

            var apiResourceList = await ToApiResourceList(apiList).ConfigureAwait(false);
            var apiScopeResourceList = await ToApiScopeList(apiScopeList).ConfigureAwait(false);
            var identityResourceList = await ToIdentityResourceList(identityList).ConfigureAwait(false);

            return new Resources
            {
                ApiResources = apiResourceList.ToList(),
                IdentityResources = identityResourceList.ToList(),
                ApiScopes = apiScopeResourceList.ToList()
            };
        }

        private async Task<IEnumerable<ApiResource>> ToApiResourceList(IEnumerable<Entity.ProtectResource> apiList)
        {
            foreach (var api in apiList)
            {
                api.ApiClaims = await _session.GetSubEntitiesAsync(api.ApiClaims).ConfigureAwait(false);
                api.ApiScopes = await _session.GetSubEntitiesAsync(api.ApiScopes).ConfigureAwait(false);
                api.Secrets = await _session.GetSubEntitiesAsync(api.Secrets).ConfigureAwait(false);
                api.Resources = await _session.GetSubEntitiesAsync(api.Resources).ConfigureAwait(false);
                api.Properties = await _session.GetSubEntitiesAsync(api.Properties).ConfigureAwait(false);
            }

            return apiList
                .Select(a => a.ToApi());
        }

        private async Task<IEnumerable<IdentityServer4.Models.ApiScope>> ToApiScopeList(IEnumerable<Entity.ApiScope> apiScopeList)
        {
            foreach (var apiScope in apiScopeList)
            {
                apiScope.ApiScopeClaims = await _session.GetSubEntitiesAsync(apiScope.ApiScopeClaims).ConfigureAwait(false);
                apiScope.Resources = await _session.GetSubEntitiesAsync(apiScope.Resources).ConfigureAwait(false);
                apiScope.Properties = await _session.GetSubEntitiesAsync(apiScope.Properties).ConfigureAwait(false);
            }

            return apiScopeList
                .Select(a => a.ToApiScope());
        }

        private async Task<IEnumerable<IdentityResource>> ToIdentityResourceList(IEnumerable<Entity.IdentityResource> identityList)
        {
            foreach (var identity in identityList)
            {
                identity.IdentityClaims = await _session.GetSubEntitiesAsync(identity.IdentityClaims).ConfigureAwait(false);
                identity.Resources = await _session.GetSubEntitiesAsync(identity.Resources).ConfigureAwait(false);
                identity.Properties = await _session.GetSubEntitiesAsync(identity.Properties).ConfigureAwait(false);
            }

            return identityList
                .Select(a => a.ToIdentity());
        }

    }
}
