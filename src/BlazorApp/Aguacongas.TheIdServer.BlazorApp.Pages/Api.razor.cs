using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Api : IDisposable
    {
        protected override string Expand => $"{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.Scopes)},{nameof(ProtectResource.Scopes)}/{nameof(ProtectResource.ApiScopeClaims)},{nameof(ProtectResource.Scopes)}/{nameof(ProtectResource.Resources)},{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}";

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

        protected override Task<ProtectResource> Create()
        {
            var scope = new ApiScope
            {
                ApiScopeClaims = new List<ApiScopeClaim>(),
                Resources = new List<ApiScopeLocalizedResource>()
            };
            EntityCreated(scope);
            return Task.FromResult(new ProtectResource
            {
                Secrets = new List<ApiSecret>(),
                Scopes = new List<ApiScope>()
                {
                    scope
                },
                ApiClaims = new List<ApiClaim>(),
                Properties = new List<ApiProperty>(),
                Resources = new List<ApiLocalizedResource>()
            });
        }

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is ProtectResource api)
            {
                api.ApiClaims = null;
                api.Properties = null;
                api.Scopes = null;
                api.Secrets = null;
                api.Resources = null;
            }
            if (entity is ApiScope scope)
            {
                scope.ApiScopeClaims = null;
                scope.Resources = null;
            }
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is ProtectResource api)
            {
                Model.Id = api.Id;
            }
            if (entity is IApiSubEntity subEntity)
            {
                subEntity.ApiId = Model.Id;
            }
            if (entity is ApiScope scope)
            {
                scope.Id = Guid.NewGuid().ToString();
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
            if (entity is ApiSecret secret && secret.Id == null && secret.Type == "ShareSecret")
            {
                secret.Value = secret.Value.Sha256();
            }
        }

        protected override void OnStateChange(ModificationKind kind, object entity)
        {
            base.OnStateChange(kind, entity);
            if (State == null)
            {
                return;
            }
            var type = entity.GetType();
            if (type == typeof(ApiClaim))
            {
                OnStateChange(kind, State.ApiClaims, entity);
                return;
            }
            if (type == typeof(ApiProperty))
            {
                OnStateChange(kind, State.Properties, entity);
                return;
            }
            if (type == typeof(ApiScope))
            {
                OnStateChange(kind, State.Scopes, entity);
                return;
            }
            if (type == typeof(ApiSecret))
            {
                OnStateChange(kind, State.Secrets, entity);
                return;
            }
            if (type == typeof(ApiScopeClaim))
            {
                var scopeClaim = entity as ApiScopeClaim;
                var scope = State.Scopes.FirstOrDefault(s => s.Id == scopeClaim.ApiScpopeId);
                if (scope != null)
                {
                    OnStateChange(kind, scope.ApiScopeClaims, entity);
                }
            }
        }

        protected override ProtectResource CloneModel(ProtectResource entity)
        {
            var clone = base.CloneModel(entity);
            clone.Properties = clone.Properties.ToList();
            clone.Resources = clone.Resources.ToList();
            clone.Scopes = clone.Scopes.ToList();
            foreach(var scope in clone.Scopes)
            {
                scope.ApiScopeClaims = scope.ApiScopeClaims.ToList();
                scope.Resources = scope.Resources.ToList();
            }
            clone.Secrets = clone.Secrets.ToList();
            clone.ApiClaims = clone.ApiClaims.ToList();
            return clone;
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

        protected override Task OnFilterChanged(string term)
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
                        .Where(c => c.Type != null && c.Type.Contains(term))
                        .ToList();
                }
            }
            Model.Properties = State.Properties
                .Where(p => (p.Key != null && p.Key.Contains(term)) || (p.Value != null && p.Value.Contains(term)))
                .ToList();

            AddEmpyClaimsTypes();

            return Task.CompletedTask;
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
                ApiScopeClaims = claims,
                Resources = new List<ApiScopeLocalizedResource>()
            };
            claim.ApiScpope = scope;
            return scope;
        }

        private ApiProperty CreateProperty()
            => new ApiProperty();

        private Task AddResource(EntityResourceKind kind)
        {
            var entity = new ApiLocalizedResource
            {
                ResourceKind = kind
            };
            Model.Resources.Add(entity);
            HandleModificationState.EntityCreated(entity);
            return Task.CompletedTask;
        }

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
