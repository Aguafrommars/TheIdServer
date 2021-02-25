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
                            .Include("ApiClaims[].Id")
                            .Include("Secrets[].Id")
                            .Include("Properties[].Id")
                            .Include("Resources[].Id")
                            .Include("ApiScopes[].Id")
                        where apiResourceNames.Contains(api.Id)
                        select api;

            var apiList = await query.ToListAsync().ConfigureAwait(false);
            return await ToApiResourceList(apiList).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var query = from apiScope in _session.Query<Entity.ApiScope>()
                            .Include("Apis[].Id")
                        where scopeNames.Contains(apiScope.Id)
                        select apiScope;

            var apiScopeList = await query.ToListAsync().ConfigureAwait(false);
            var dictionary = new Dictionary<string, Entity.ProtectResource>();
            foreach (var apiScope in apiScopeList)
            {
                var apiApiScopeList = await _session.LoadAsync<Entity.ApiApiScope>(apiScope.Apis.Select(a => a.Id)).ConfigureAwait(false);
                var apiList = await _session.LoadAsync<Entity.ProtectResource>(apiApiScopeList.Select(s => $"{nameof(Entity.ProtectResource).ToLowerInvariant()}/{s.Value.ApiId}")).ConfigureAwait(false);
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
            var query = from scope in _session.Query<Entity.ApiScope>()
                            .Include("ApiScopeClaims[].Id")
                            .Include("Properties[].Id")
                            .Include("Resources[].Id")
                        where scopeNames.Contains(scope.Id)
                        select scope;

            var apiScopeList = await query.ToListAsync().ConfigureAwait(false);
            return await ToApiScopeList(apiScopeList).ConfigureAwait(false);
        }


        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var query = from identity in _session.Query<Entity.IdentityResource>()
                            .Include("IdentityClaims[].Id")
                            .Include("Properties[].Id")
                            .Include("Resources[].Id")
                        where scopeNames.Contains(identity.Id)
                        select identity;

            var identityList = await query.ToListAsync().ConfigureAwait(false);
            return await ToIdentityResourceList(identityList).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiQuery = from api in _session.Query<Entity.ProtectResource>()
                            .Include("ApiClaims[].Id")
                            .Include("Secrets[].Id")
                            .Include("Properties[].Id")
                            .Include("Resources[].Id")
                            .Include("ApiScopes[].Id")
                        select api;

            var apiScopeQuery = from scope in _session.Query<Entity.ApiScope>()
                            .Include("ApiScopeClaims[].Id")
                            .Include("Properties[].Id")
                            .Include("Resources[].Id")
                        select scope;

            var identityQuery = from identity in _session.Query<Entity.IdentityResource>()
                            .Include("IdentityClaims[].Id")
                            .Include("Properties[].Id")
                            .Include("Resources[].Id")
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
                var claimList = await _session.LoadAsync<Entity.ApiClaim>(api.ApiClaims.Select(c => c.Id)).ConfigureAwait(false);
                api.ApiClaims = claimList.Select(e => e.Value).ToList();

                var secretList = await _session.LoadAsync<Entity.ApiSecret>(api.Secrets.Select(s => s.Id)).ConfigureAwait(false);
                api.Secrets = secretList.Select(e => e.Value).ToList();

                var propertyList = await _session.LoadAsync<Entity.ApiProperty>(api.Properties.Select(p => p.Id)).ConfigureAwait(false);
                api.Properties = propertyList.Select(e => e.Value).ToList();

                var apiScopeList = await _session.LoadAsync<Entity.ApiApiScope>(api.ApiScopes.Select(s => s.Id)).ConfigureAwait(false);
                api.ApiScopes = apiScopeList.Select(e => e.Value).ToList();

                var resourceList = await _session.LoadAsync<Entity.ApiLocalizedResource>(api.Resources.Select(s => s.Id)).ConfigureAwait(false);
                api.Resources = resourceList.Select(e => e.Value).ToList();
            }

            return apiList
                .Select(a => a.ToApi());
        }

        private async Task<IEnumerable<IdentityServer4.Models.ApiScope>> ToApiScopeList(IEnumerable<Entity.ApiScope> apiScopeList)
        {
            foreach (var apiScope in apiScopeList)
            {
                var claimList = await _session.LoadAsync<Entity.ApiScopeClaim>(apiScope.ApiScopeClaims.Select(c => c.Id)).ConfigureAwait(false);
                apiScope.ApiScopeClaims = claimList.Select(e => e.Value).ToList();

                var resourceList = await _session.LoadAsync<Entity.ApiScopeLocalizedResource>(apiScope.Resources.Select(s => s.Id)).ConfigureAwait(false);
                apiScope.Resources = resourceList.Select(e => e.Value).ToList();

                var propertyList = await _session.LoadAsync<Entity.ApiScopeProperty>(apiScope.Properties.Select(p => p.Id)).ConfigureAwait(false);
                apiScope.Properties = propertyList.Select(e => e.Value).ToList();

            }

            return apiScopeList
                .Select(a => a.ToApiScope());
        }

        private async Task<IEnumerable<IdentityResource>> ToIdentityResourceList(IEnumerable<Entity.IdentityResource> identityList)
        {
            foreach (var identity in identityList)
            {
                var claimList = await _session.LoadAsync<Entity.IdentityClaim>(identity.IdentityClaims.Select(c => c.Id)).ConfigureAwait(false);
                identity.IdentityClaims = claimList.Select(e => e.Value).ToList();

                var resourceList = await _session.LoadAsync<Entity.IdentityLocalizedResource>(identity.Resources.Select(s => s.Id)).ConfigureAwait(false);
                identity.Resources = resourceList.Select(e => e.Value).ToList();

                var propertyList = await _session.LoadAsync<Entity.IdentityProperty>(identity.Properties.Select(p => p.Id)).ConfigureAwait(false);
                identity.Properties = propertyList.Select(e => e.Value).ToList();

            }

            return identityList
                .Select(a => a.ToIdentity());
        }

    }
}
