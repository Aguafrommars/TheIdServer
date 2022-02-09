// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public abstract class EntityResourcesComponentBase<T> : ComponentBase where T:IEntityResource
    {
        [Parameter]
        public ICollection<T> Collection { get; set; }

        [Parameter]
        public EntityResourceKind ResourceKind { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }
        
        protected Task OnDeleteResource(IEntityResource resource)
        {
            Collection.Remove((T)resource);
            HandleModificationState.EntityDeleted((T)resource);
            return Task.CompletedTask;
        }
    }
}
