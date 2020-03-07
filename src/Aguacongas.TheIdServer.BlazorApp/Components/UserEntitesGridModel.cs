using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public class UserEntitesGridModel<T> : ComponentBase
    {
        protected GridState GridState { get; } = new GridState();

        [Parameter]
        public IEnumerable<T> Model { get; set; }

        [Parameter]
        public EventCallback<T> DeleteEntityClicked { get; set; }

        protected void OnDeleteEntityClicked(T entity)
        {
            DeleteEntityClicked.InvokeAsync(entity);
        }
    }
}
