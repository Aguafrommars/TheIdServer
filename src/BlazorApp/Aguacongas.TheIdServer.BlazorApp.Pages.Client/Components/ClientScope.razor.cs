// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientScope
    {
        private bool _isReadOnly;
        private string _href;

        [Parameter]
        public entity.Client Model { get; set; }

        private static readonly ScopeComparer _comparer = new();
        private IEnumerable<Scope> _filterScopes;
        private readonly PageRequest _idPageRequest = new()
        {
            Select = $"{nameof(entity.IdentityResource.Id)},{nameof(entity.IdentityResource.DisplayName)}",
            Expand = nameof(entity.IdentityResource.Resources),
            Take = 5
        };
        private readonly PageRequest _scopeRequest = new()
        {
            Select = $"{nameof(entity.ApiScope.Id)},{nameof(entity.ApiScope.DisplayName)}",
            Expand = nameof(entity.ApiScope.Resources),
            Take = 5
        };

        protected override bool IsReadOnly => _isReadOnly;

        protected override string PropertyName => "Scope";

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync().ConfigureAwait(false);
            _isReadOnly = Entity.Id != null;
            if (_isReadOnly)
            {
                await SetHrefAsync().ConfigureAwait(false);
            }
        }

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term, CancellationToken cancellationToken)
        {
            var identityResponse = await GetIdentityScopeAsync(term, cancellationToken).ConfigureAwait(false);
            _scopeRequest.Filter = $"contains({nameof(entity.ApiScope.Id)},'{term}') or contains({nameof(entity.ApiScope.DisplayName)},'{term}')";
            var apiScopeResponse = await _apiScopeStore.GetAsync(_scopeRequest, cancellationToken).ConfigureAwait(false);

            var culture = CultureInfo.CurrentCulture.Name;
            _filterScopes = identityResponse.Items.Select(i => new Scope
            {
                Value = i.Id,
                Description = i.Resources.FirstOrDefault(r => r.ResourceKind == entity.EntityResourceKind.DisplayName && r.CultureId == culture)?.Value
                        ?? i.DisplayName,
                IsIdentity = true
            })
                .Union(apiScopeResponse.Items.Select(s => new Scope
                {
                    Value = s.Id,
                    Description = s.Resources.FirstOrDefault(r => r.ResourceKind == entity.EntityResourceKind.DisplayName && r.CultureId == culture)?.Value
                        ?? s.DisplayName
                }))
                .Distinct(_comparer)
                .Where(s => Model.AllowedScopes != null && !Model.AllowedScopes.Any(cs => cs.Id != null && s.Value == cs.Scope))
                .Take(5)
                .OrderBy(r => r.Value);

            return Array.Empty<string>();
        }

        private async Task<PageResponse<IdentityResource>> GetIdentityScopeAsync(string term, CancellationToken cancellationToken)
        {
            if (Model.AllowedGrantTypes.All(g => g.GrantType == "client_credentials"))
            {
                return new PageResponse<IdentityResource>
                {
                    Items = Array.Empty<IdentityResource>(),
                    Count = 0
                };
            }

            _idPageRequest.Filter = $"contains({nameof(entity.IdentityResource.Id)},'{term}') or contains({nameof(entity.IdentityResource.DisplayName)},'{term}')";
            var identityResponse = await _identityStore.GetAsync(_idPageRequest, cancellationToken).ConfigureAwait(false);
            return identityResponse;
        }

        protected override void SetValue(string inputValue)
        {
            Entity.Scope = inputValue;
        }

        private sealed class ScopeComparer : IEqualityComparer<Scope>
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

        private async Task SetHrefAsync()
        {
            var identityResponse = await _identityStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(entity.IdentityResource.Id)} eq '{Entity.Scope}'",
                Take = 0
            }).ConfigureAwait(false);
            if (identityResponse.Count != 0)
            {
                _href = $"/identityresource/{Entity.Scope}";
                return;
            }

            var apiScopeResponse = await _apiScopeStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(entity.ApiScope.Id)} eq '{Entity.Scope}'",
                Take = 0
            }).ConfigureAwait(false);
            if (apiScopeResponse.Count != 0)
            {
                _href = $"/apiscope/{Entity.Scope}";
            }
        }
    }
}
