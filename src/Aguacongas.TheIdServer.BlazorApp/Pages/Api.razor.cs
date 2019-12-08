using Aguacongas.IdentityServer.Store.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Api
    {
        protected override string Expand => "Secrets,Scopes,Scopes/ApiScopeClaims,ApiClaims,Properties";

        protected override bool NonEditable => Model.NonEditable;

        protected override string BackUrl => "apis";

        public override int Compare(Type x, Type y)
        {
            if (x == typeof(ApiScopeClaim))
            {
                return 1;
            }
            if (y == typeof(ApiScopeClaim))
            {
                return -1;
            }
            return base.Compare(x, y);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            AddEmpyClaimsTypes();
        }

        protected override ProtectResource Create()
        {
            var scope = new ApiScope
            {
                ApiScopeClaims = new List<ApiScopeClaim>()
            };
            EntityCreated(scope);
            return new ProtectResource
            {
                Secrets = new List<ApiSecret>(),
                Scopes = new List<ApiScope>()
            {
                scope
            },
                ApiClaims = new List<ApiClaim>(),
                Properties = new List<ApiProperty>()
            };
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is IApiSubEntity subEntity)
            {
                subEntity.ApiId = Model.Id;
            }
            if (entity is ApiScopeClaim apiScopeClaim)
            {
                if (apiScopeClaim.ApiScpope == null)
                {
                    throw new InvalidOperationException("ApiScopeClaim.ApiScope property cannot be null.");
                }
                if (apiScopeClaim.ApiScpope.Id == null)
                {
                    throw new InvalidOperationException("ApiScopeClaim.ApiScope.Id property cannot be null.");
                }
                apiScopeClaim.ApiScpopeId = apiScopeClaim.ApiScpope.Id;
                apiScopeClaim.ApiScpope = null;
            }
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is ProtectResource api)
            {
                api.ApiClaims = null;
                api.Properties = null;
                api.Scopes = null;
                api.Secrets = null;
            }
            if (entity is ApiScope scope)
            {
                scope.ApiScopeClaims = null;
            }
            if (entity is ApiSecret secret && secret.Id == null)
            {
                secret.Value = secret.Value.Sha256();
            }
        }

        protected override void SetModelEntityId(Type entityType, object result)
        {
            if (entityType == typeof(ApiScope))
            {
                var scope = result as ApiScope;
                var modelScope = Model.Scopes.SingleOrDefault(s => s.Id == null && s.Scope == scope.Scope);
                if (modelScope != null)
                {
                    modelScope.Id = scope.Id;
                }
            }
        }

        private void AddEmpyClaimsTypes()
        {
            Model.ApiClaims.Add(new ApiClaim());
            foreach (var scope in Model.Scopes)
            {
                scope.ApiScopeClaims.Add(new ApiScopeClaim());
            }
        }

        private void OnFilterChanged(string term)
        {
            Model.ApiClaims = State.ApiClaims
                .Where(c => c.Type != null && c.Type.Contains(term))
                .ToList();
            Model.Secrets = State.Secrets
                .Where(s => (s.Description != null && s.Description.Contains(term)) || (s.Type != null && s.Type.Contains(term)))
                .ToList();
            Model.Scopes = State.Scopes
                .Where(s => (s.Description != null && s.Description.Contains(term)) ||
                    (s.DisplayName != null && s.DisplayName.Contains(term)) ||
                    (s.Scope != null && s.Scope.Contains(term)))
                .ToList();
            foreach (var scope in Model.Scopes)
            {
                var stateScope = State.Scopes.FirstOrDefault(s => s.Scope == scope.Scope);
                if (stateScope != null)
                {
                    scope.ApiScopeClaims = stateScope.ApiScopeClaims
                        .Where(c => (c.Type != null && c.Type.Contains(term)))
                        .ToList();
                }
            }
            Model.Properties = State.Properties
                .Where(p => (p.Key != null && p.Key.Contains(term)) || (p.Value != null && p.Value.Contains(term)))
                .ToList();

            AddEmpyClaimsTypes();
        }

        private void OnAddSecretClicked()
        {
            var secret = new ApiSecret
            {
                Type = "SharedSecret"
            };
            Model.Secrets.Add(secret);
            EntityCreated(secret);
            StateHasChanged();
        }

        private void OnDeleteSecretClicked(ApiSecret secret)
        {
            Model.Secrets.Remove(secret);
            EntityDeleted(secret);
            StateHasChanged();
        }

        private void OnClaimTypeValueChanged(ApiClaim claim)
        {
            EntityCreated(claim);
            Model.ApiClaims.Add(new ApiClaim());
            StateHasChanged();
        }

        private void OnClaimTypeDeleted(ApiClaim claim)
        {
            Model.ApiClaims.Remove(claim);
            EntityDeleted(claim);
            StateHasChanged();
        }

        private void OnAddScopeClicked()
        {
            var claims = new List<ApiScopeClaim>()
            {
                new ApiScopeClaim()
            };
            var scope = new ApiScope
            {
                ApiScopeClaims = claims
            };
            Model.Scopes.Add(scope);
            EntityCreated(scope);
            StateHasChanged();
        }

        private void OnDeleteScopeClicked(ApiScope scope)
        {
            Model.Scopes.Remove(scope);
            EntityDeleted(scope);
            StateHasChanged();
        }

        private void OnScopeClaimDeleted(ApiScope scope, ApiScopeClaim claim)
        {
            scope.ApiScopeClaims.Remove(claim);
            EntityDeleted(claim);
            StateHasChanged();
        }

        private void OnScopeClaimValueChanged(ApiScope scope, ApiScopeClaim claim)
        {
            claim.ApiScpope = scope;
            EntityCreated(claim);
            scope.ApiScopeClaims.Add(new ApiScopeClaim());
            StateHasChanged();
        }

        private void OnAddPropertyClicked()
        {
            var property = new ApiProperty();
            Model.Properties.Add(property);
            EntityCreated(property);
            StateHasChanged();
        }

        private void OnDeletePropertyClicked(ApiProperty property)
        {
            Model.Properties.Remove(property);
            EntityDeleted(property);
            StateHasChanged();
        }
    }
}
