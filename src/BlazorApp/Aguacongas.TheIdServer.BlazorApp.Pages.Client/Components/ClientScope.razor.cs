// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Pages.Client.Extentions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityNS = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientScope
    {
        private bool _isReadOnly;
        private string _href;

        [Parameter]
        public EntityNS.Client Model { get; set; }

        private static readonly ScopeComparer _comparer = new();
        private IEnumerable<Scope> _filterScopes;
        private readonly PageRequest _idPageRequest = new()
        {
            Select = $"{nameof(EntityNS.IdentityResource.Id)},{nameof(EntityNS.IdentityResource.DisplayName)}",
            Expand = nameof(EntityNS.IdentityResource.Resources),
            Take = 5
        };
        private readonly PageRequest _scopeRequest = new()
        {
            Select = $"{nameof(EntityNS.ApiScope.Id)},{nameof(EntityNS.ApiScope.DisplayName)}",
            Expand = nameof(EntityNS.ApiScope.Resources),
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
            var apiScopeResponse = await GetApiScopeAsync(term, cancellationToken).ConfigureAwait(false);

            var culture = CultureInfo.CurrentCulture.Name;
            _filterScopes = identityResponse.Items.Select(i => new Scope
                {
                    Value = i.Id,
                    Description = i.Resources.FirstOrDefault(r => r.ResourceKind == EntityNS.EntityResourceKind.DisplayName && r.CultureId == culture)?.Value
                            ?? i.DisplayName,
                    IsIdentity = true
                })
                .Union(apiScopeResponse.Items.Select(s => new Scope
                {
                    Value = s.Id,
                    Description = s.Resources.FirstOrDefault(r => r.ResourceKind == EntityNS.EntityResourceKind.DisplayName && r.CultureId == culture)?.Value
                        ?? s.DisplayName
                }))
                .Distinct(_comparer)
                .Where(s => Model.AllowedScopes != null && !Model.AllowedScopes.Any(cs => cs.Id != null && s.Value == cs.Scope))
                .Take(5)
                .OrderBy(r => r.Value);

            return Array.Empty<string>();
        }

        private async Task<PageResponse<EntityNS.IdentityResource>> GetIdentityScopeAsync(string term, CancellationToken cancellationToken)
        {
            if (Model.IsClientCredentialOnly())
            {
                return new PageResponse<EntityNS.IdentityResource>
                {
                    Items = Array.Empty<EntityNS.IdentityResource>(),
                    Count = 0
                };
            }

            _idPageRequest.Filter = $"contains({nameof(EntityNS.IdentityResource.Id)},'{term}') or contains({nameof(EntityNS.IdentityResource.DisplayName)},'{term}')";
            var identityResponse = await _identityStore.GetAsync(_idPageRequest, cancellationToken).ConfigureAwait(false);
            return identityResponse;
        }

        private async Task<PageResponse<EntityNS.ApiScope>> GetApiScopeAsync(string term, CancellationToken cancellationToken)
        {
            if (Model.ProtocolType != "oidc")
            {
                return new PageResponse<EntityNS.ApiScope>
                {
                    Items = Array.Empty<EntityNS.ApiScope>(),
                    Count = 0
                };
            }

            _scopeRequest.Filter = $"contains({nameof(EntityNS.ApiScope.Id)},'{term}') or contains({nameof(EntityNS.ApiScope.DisplayName)},'{term}')";
            return await _apiScopeStore.GetAsync(_scopeRequest, cancellationToken).ConfigureAwait(false);
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
                Filter = $"{nameof(EntityNS.IdentityResource.Id)} eq '{Entity.Scope}'",
                Take = 0
            }).ConfigureAwait(false);
            if (identityResponse.Count != 0)
            {
                _href = $"/identityresource/{Entity.Scope}";
                return;
            }

            var apiScopeResponse = await _apiScopeStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(EntityNS.ApiScope.Id)} eq '{Entity.Scope}'",
                Take = 0
            }).ConfigureAwait(false);
            if (apiScopeResponse.Count != 0)
            {
                _href = $"/apiscope/{Entity.Scope}";
            }
        }
    }
}
