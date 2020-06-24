using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiScopes
    {
        private IEnumerable<Entity.ApiScope> Scopes => Collection.Where(s => s.Id == null || (s.Description != null && s.Description.Contains(HandleModificationState.FilterTerm)) ||
           (s.DisplayName != null && s.DisplayName.Contains(HandleModificationState.FilterTerm)) ||
           (s.Id != null && s.Id.Contains(HandleModificationState.FilterTerm)));

        [Parameter]
        public ICollection<Entity.ApiScope> Collection { get; set; }

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
            if (entity is Entity.ApiScope)
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
