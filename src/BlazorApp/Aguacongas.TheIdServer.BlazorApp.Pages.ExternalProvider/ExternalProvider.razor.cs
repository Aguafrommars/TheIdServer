// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider
{
    public partial class ExternalProvider
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Used in component")]
        private ProviderOptionsBase _optionsComponent;

        protected override string Expand => $"{nameof(Models.ExternalProvider.ClaimTransformations)}";

        protected override bool NonEditable => false;

        protected override string BackUrl => "providers";

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            // no nav
        }

        protected override Task<Models.ExternalProvider> Create()
        {
            return Task.FromResult(new Models.ExternalProvider
            {
                ClaimTransformations = new List<ExternalClaimTransformation>()
            });
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            var providerKindsResponse = await _providerKindStore.GetAsync(new PageRequest()).ConfigureAwait(false);
            Model.Kinds = providerKindsResponse.Items;
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is Models.ExternalProvider provider)
            {
                provider.SerializedOptions = _optionsComponent.SerializeOptions();
            }
        }

        protected override void OnEntityUpdated(Type entityType, IEntityId entityModel)
        {
            if (entityType != typeof(ExternalClaimTransformation))
            {
                base.OnEntityUpdated(typeof(Models.ExternalProvider), Model);
            }
            else
            {
                base.OnEntityUpdated(entityType, entityModel);
            }    
        }

        private ExternalClaimTransformation CreateTransformation()
        {
            return new ExternalClaimTransformation
            {
                Scheme = Model.Id
            };
        }

        private void KindChanded(string value)
        {
            Model.KindName = value;
            Model.Options = null;
            InvokeAsync(StateHasChanged);
        }
    }
}
