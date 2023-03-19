// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        protected override void OnInitialized()
        {
            if (HandleModificationState != null)
            {
                HandleModificationState.OnFilterChange += HandleModificationState_OnFilterChange;
                HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
            }
            GridState.OnHeaderClicked += GridState_OnHeaderClicked;
            base.OnInitialized();
        }

        private Task GridState_OnHeaderClicked(Models.SortEventArgs arg)
        {
            if (string.IsNullOrEmpty(arg.OrderBy))
            {
                return Task.CompletedTask;
            }
            OnStort(arg);
            return InvokeAsync(StateHasChanged);
        }

        protected virtual void OnStort(SortEventArgs arg)
        {
            Collection = Collection.AsQueryable().Sort(arg.OrderBy).ToList();
        }

        private void HandleModificationState_OnStateChange(ModificationKind kind, object entity)
        {
            if (entity is T && (kind == ModificationKind.Add || kind == ModificationKind.Delete))
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string term)
        {
            StateHasChanged();
        }

        protected void OnDeleteEntityClicked(T entity)
        {
            Collection.Remove(entity);
            HandleModificationState.EntityDeleted(entity);
        }
    }
}
