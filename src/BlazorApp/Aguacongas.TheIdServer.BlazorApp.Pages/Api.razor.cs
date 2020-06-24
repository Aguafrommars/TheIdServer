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
                scope.Id = scope.Id ?? Guid.NewGuid().ToString();
            }
            if (entity is ApiScopeClaim apiScopeClaim)
            {
                if (apiScopeClaim.ApiScope == null)
                {
                    throw new InvalidOperationException("ApiScopeClaim.ApiScope property cannot be null.");
                }
                if (apiScopeClaim.ApiScope.Id == null)
                {
                    throw new InvalidOperationException("ApiScopeClaim.ApiScope.Id property cannot be null.");
                }
                apiScopeClaim.ApiScopeId = apiScopeClaim.ApiScope.Id;
                apiScopeClaim.ApiScope = null;
            }
            if (entity is ApiScopeLocalizedResource apiScopeResource)
            {
                if (apiScopeResource.ApiScope == null)
                {
                    throw new InvalidOperationException("apiScopeResource.ApiScope property cannot be null.");
                }
                if (apiScopeResource.ApiScope.Id == null)
                {
                    throw new InvalidOperationException("apiScopeResource.ApiScope.Id property cannot be null.");
                }
                apiScopeResource.ApiScopeId = apiScopeResource.ApiScope.Id;
                apiScopeResource.ApiScope = null;
            }
            if (entity is ApiSecret secret && secret.Id == null && secret.Type == "ShareSecret")
            {
                secret.Value = secret.Value.Sha256();
            }
        }

        private void OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            var scope = Model.Scopes.FirstOrDefault();
            if (IsNew && scope != null)
            {
                if (scope.Id == null && e.FieldIdentifier.FieldName == "Id")
                {
                    scope.Id = Model.Id;
                }
                if (scope.DisplayName == null && e.FieldIdentifier.FieldName == "DisplayName")
                {
                    scope.DisplayName = Model.DisplayName;
                }
            }
        }        

        private ApiSecret CreateSecret()
            =>  new ApiSecret
                {
                    Type = "SharedSecret"
                };

        private ApiScope CreateApiScope()
        {
            var scope = new ApiScope
            {
                ApiScopeClaims = new List<ApiScopeClaim>(),
                Resources = new List<ApiScopeLocalizedResource>()
            };
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
