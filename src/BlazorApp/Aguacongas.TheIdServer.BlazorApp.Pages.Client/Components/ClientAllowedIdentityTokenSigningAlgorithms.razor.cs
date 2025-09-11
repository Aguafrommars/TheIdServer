// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientAllowedIdentityTokenSigningAlgorithms
    {
        private IEnumerable<Entity.ClientAllowedIdentityTokenSigningAlgorithm> Algorithms => Model.AllowedIdentityTokenSigningAlgorithms.Where(a => a.Algorithm != null && a.Algorithm.Contains(HandleModificationState.FilterTerm));
        private Entity.ClientAllowedIdentityTokenSigningAlgorithm _algorithm = new Entity.ClientAllowedIdentityTokenSigningAlgorithm();

        [Parameter]
        public Entity.Client Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected override void OnInitialized()
        {
            HandleModificationState.OnFilterChange += HandleModificationState_OnFilterChange;
            HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
            base.OnInitialized();
        }

        private void HandleModificationState_OnStateChange(ModificationKind kind, object entity)
        {
            if (entity is Entity.ClientAllowedIdentityTokenSigningAlgorithm)
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string obj)
        {
            StateHasChanged();
        }

        private void OnAlgorithmDeletedClicked(Entity.ClientAllowedIdentityTokenSigningAlgorithm algorithm)
        {
            Model.AllowedIdentityTokenSigningAlgorithms.Remove(algorithm);
            HandleModificationState.EntityDeleted(algorithm);
        }

        private void OnAlgorithmValueChanged(Entity.ClientAllowedIdentityTokenSigningAlgorithm algorithm)
        {
            Model.AllowedIdentityTokenSigningAlgorithms.Add(algorithm);
            _algorithm = new Entity.ClientAllowedIdentityTokenSigningAlgorithm();
            algorithm.Id = Guid.NewGuid().ToString();
            HandleModificationState.EntityCreated(algorithm);
        }
    }
}
