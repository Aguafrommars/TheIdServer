// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Identity
{
    public partial class Identity
    {
        protected override string Expand => $"{nameof(IdentityResource.IdentityClaims)},{nameof(IdentityResource.Properties)},{nameof(IdentityResource.Resources)}";

        protected override bool NonEditable => Model.NonEditable;

        protected override string BackUrl => "identities";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        protected override Task<IdentityResource> Create()
        {
            return Task.FromResult(new IdentityResource
            {
                IdentityClaims = new List<IdentityClaim>(),
                Properties = new List<IdentityProperty>(),
                Resources = new List<IdentityLocalizedResource>()
            });
        }

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is IdentityResource identity)
            {
                identity.IdentityClaims = null;
                identity.Properties = null;
                identity.Resources = null;
            }
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is IdentityResource identity)
            {
                Model.Id = identity.Id;
            }
            if (entity is IIdentitySubEntity subEntity)
            {
                subEntity.IdentityId = Model.Id;
            }
        }

        private IdentityProperty CreateProperty()
            => new IdentityProperty();

        private Task AddResource(EntityResourceKind kind)
        {
            var entity = new IdentityLocalizedResource
            {
                ResourceKind = kind
            };
            Model.Resources.Add(entity);
            HandleModificationState.EntityCreated(entity);
            return Task.CompletedTask;
        }
    }
}
