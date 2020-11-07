// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientScope
    {
        private bool _isReadOnly;

        [Parameter]
        public entity.Client Model { get; set; }

        private static readonly ScopeComparer _comparer = new ScopeComparer();
        private IEnumerable<Scope> _filterScopes;
        private readonly PageRequest _idPageRequest = new PageRequest
        {
            Select = $"{nameof(entity.IdentityResource.Id)},{nameof(entity.IdentityResource.DisplayName)}",
            Expand = nameof(entity.IdentityResource.Resources),
            Take = 5
        };
        private readonly PageRequest _scopeRequest = new PageRequest
        {
            Select = $"{nameof(entity.ApiScope.Id)},{nameof(entity.ApiScope.DisplayName)}",
            Expand = nameof(entity.ApiScope.Resources),
            Take = 5
        };

        protected override bool IsReadOnly => _isReadOnly;

        protected override string PropertyName => "Scope";

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _isReadOnly = Entity.Id != null;
        }

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term)
        {
            _idPageRequest.Filter = $"contains({nameof(entity.IdentityResource.Id)},'{term}') or contains({nameof(entity.IdentityResource.DisplayName)},'{term}')";
            _scopeRequest.Filter = $"contains({nameof(entity.ApiScope.Id)},'{term}') or contains({nameof(entity.ApiScope.DisplayName)},'{term}')";
            var identityResponse = _identityStore.GetAsync(_idPageRequest);
            var apiScopeResponse = _apiScopeStore.GetAsync(_scopeRequest);

            await Task.WhenAll(identityResponse, apiScopeResponse)
                .ConfigureAwait(false);
            var culture = CultureInfo.CurrentCulture.Name;
            _filterScopes = identityResponse.Result.Items.Select(i => new Scope
            {
                Value = i.Id,
                Description = i.Resources.FirstOrDefault(r => r.ResourceKind == entity.EntityResourceKind.DisplayName && r.CultureId == culture)?.Value
                    ?? i.DisplayName,
                IsIdentity = true
            })
                .Union(apiScopeResponse.Result.Items.Select(s => new Scope
                {
                    Value = s.Id,
                    Description = s.Resources.FirstOrDefault(r => r.ResourceKind == entity.EntityResourceKind.DisplayName && r.CultureId == culture)?.Value
                        ?? s.DisplayName
                }))
                .Distinct(_comparer)
                .Where(s => !Model.AllowedScopes.Any(cs => cs.Id != null && s.Value == cs.Scope))
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
