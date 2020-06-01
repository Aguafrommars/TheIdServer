using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public class EntitesGridModel<T> : ComponentBase where T: IEntityId
    {
        [Inject]
        protected IStringLocalizerAsync<EntitesGridModel<T>> Localizer { get; set; }

        protected GridState GridState { get; } = new GridState();

        [Parameter]
        public ICollection<T> Collection { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected void OnDeleteEntityClicked(T entity)
        {
            Collection.Remove(entity);
            HandleModificationState.EntityDeleted(entity);
            StateHasChanged();
        }
    }
}
