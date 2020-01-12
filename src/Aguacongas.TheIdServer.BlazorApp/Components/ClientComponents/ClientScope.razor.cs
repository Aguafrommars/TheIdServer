using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientScope
    {
        private bool _isReadOnly;

        [Parameter]
        public Entity.Client Model { get; set; }

        private static readonly ScopeComparer _comparer = new ScopeComparer();
        private IEnumerable<Scope> _filterScopes;
        private readonly PageRequest _idPageRequest = new PageRequest
        {
            Select = "Id,DisplayName",
            Take = 5
        };
        private readonly PageRequest _scopeRequest = new PageRequest
        {
            Select = "Scope,DisplayName",
            Take = 5
        };

        protected override bool IsReadOnly => _isReadOnly;

        protected override string PropertyName => "Scope";

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _isReadOnly = Entity.Scope != null;
        }

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term)
        {
            _idPageRequest.Filter = $"contains(Id,'{term}') or contains(DisplayName,'{term}')";
            _scopeRequest.Filter = $"contains(Scope,'{term}') or contains(DisplayName,'{term}')";
            var identityResponse = _identityStore.GetAsync(_idPageRequest);
            var apiResponse = _apiStore.GetAsync(_idPageRequest);
            var apiScopeResponse = _apiScopeStore.GetAsync(_scopeRequest);

            await Task.WhenAll(identityResponse, apiResponse, apiScopeResponse)
                .ConfigureAwait(false);


            _filterScopes = identityResponse.Result.Items.Select(i => new Scope
            {
                Value = i.Id,
                Description = i.DisplayName,
                IsIdentity = true
            })
                .Union(apiResponse.Result.Items.Select(a => new Scope
                {
                    Value = a.Id,
                    Description = a.DisplayName
                }))
                .Union(apiScopeResponse.Result.Items.Select(s => new Scope
                {
                    Value = s.Scope,
                    Description = s.DisplayName
                }))
                .Distinct(_comparer)
                .Where(s => !Model.AllowedScopes.Any(cs => s.Value == cs.Scope))
                .Take(5)
                .OrderBy(r => r.Value);

            return Array.Empty<string>();
        }

        protected override void SetValue(string inputValue)
        {
            Entity.Scope = inputValue;
        }

        private class ScopeComparer : IEqualityComparer<Scope>
        {
            public bool Equals(Scope x, Scope y)
            {
                return x.Value == y.Value && x.IsIdentity == y.IsIdentity;
            }

            public int GetHashCode(Scope obj)
            {
                return -1;
            }
        }
    }
}
