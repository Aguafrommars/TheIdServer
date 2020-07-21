using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Api
    {
        protected override string Expand => $"{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}";

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
        }

        protected override Task<ProtectResource> Create()
        {
            return Task.FromResult(new ProtectResource
            {
                Secrets = new List<ApiSecret>(),
                ApiScopes = new List<ApiApiScope>(),
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
                api.ApiScopes = null;
                api.Secrets = null;
                api.Resources = null;
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
            if (entity is ApiSecret secret && secret.Id == null && secret.Type == SecretTypes.SharedSecret)
            {
                secret.Value = secret.Value.Sha256();
            }
        }

        private ApiSecret CreateSecret()
            =>  new ApiSecret
                {
                    Type = "SharedSecret"
                };

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
    }
}
