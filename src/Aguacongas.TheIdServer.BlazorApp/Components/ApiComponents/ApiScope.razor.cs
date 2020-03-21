using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiScope
    {
        [Parameter]
        public ICollection<Entity.ApiScope> Collection { get; set; }

        [Parameter]
        public Entity.ApiScope Scope { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        private void OnDeleteScopeClicked()
        {
            Collection.Remove(Scope);
            HandleModificationState.EntityDeleted(Scope);
        }
    }
}
