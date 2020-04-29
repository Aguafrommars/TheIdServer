using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Api : IDisposable
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
            await base.OnInitializedAsync().ConfigureAwait(false);
            AddEmpyClaimsTypes();
            EditContext.OnFieldChanged += OnFieldChanged;
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
                scope.ApiScopeClaims.Add(new ApiScopeClaim { ApiScpope = scope });
            }
        }

        private void OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            var scope = Model.Scopes.FirstOrDefault();
            if (IsNew && scope != null)
            {
                if (scope.Scope == null && e.FieldIdentifier.FieldName == "Id")
                {
                    scope.Scope = Model.Id;
                }
                if (scope.DisplayName == null && e.FieldIdentifier.FieldName == "DisplayName")
                {
                    scope.DisplayName = Model.DisplayName;
                }
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

        private ApiSecret CreateSecret()
         =>  new ApiSecret
            {
                Type = "SharedSecret"
            };

        private ApiScope CreateApiScope()
        {
            var claim = new ApiScopeClaim();
            var claims = new List<ApiScopeClaim>()
            {
                claim
            };
            var scope = new ApiScope
            {
                ApiScopeClaims = claims
            };
            claim.ApiScpope = scope;
            return scope;
        }

        private ApiProperty CreateProperty()
            => new ApiProperty();


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EditContext.OnFieldChanged -= OnFieldChanged;
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
