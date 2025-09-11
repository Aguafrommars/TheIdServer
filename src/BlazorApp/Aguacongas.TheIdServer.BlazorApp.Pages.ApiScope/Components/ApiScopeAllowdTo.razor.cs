// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ApiScope.Components
{
    public partial class ApiScopeAllowdTo
    {
        private IEnumerable<Entity.ClientScope> _clients = Array.Empty<Entity.ClientScope>();
        private IEnumerable<Entity.ClientScope> Clients => _clients.Where(c => c.ClientId.Contains(HandleModificationState.FilterTerm));

        [Parameter]
        public Entity.ApiScope Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        [Inject]
        protected IAdminStore<Entity.ClientScope> ClientStore { get; set; }

        protected override void OnInitialized()
        {
            HandleModificationState.OnFilterChange += HandleModificationState_OnFilterChange;
            HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            var page = await ClientStore.GetAsync(new PageRequest
            {
                Select = nameof(Entity.Client.Id),
                Filter = $"{nameof(Entity.ClientScope.Scope)} eq '{Model.Id}'"
            }).ConfigureAwait(false);
            _clients = page.Items;
            await base.OnParametersSetAsync().ConfigureAwait(false);
        }

        private void HandleModificationState_OnStateChange(ModificationKind kind, object entity)
        {
            if (entity is Entity.ApiApiScope)
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string obj)
        {
            StateHasChanged();
        }
    }
}
