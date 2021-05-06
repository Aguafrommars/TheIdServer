// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientUrisPanel
    {
        [Parameter]
        public Entity.Client Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected override void OnInitialized()
        {
            HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
            if (Model.ProtocolType == "wsfed" && !Model.RedirectUris.Any())
            {
                var uri = new Entity.ClientUri
                {
                    Kind = Entity.UriKinds.Redirect
                };
                Model.RedirectUris.Add(uri);
                HandleModificationState.EntityCreated(uri);
            }
        }

        private void HandleModificationState_OnStateChange(ModificationKind kind, object entity)
        {
            if (kind == ModificationKind.Add && entity is Entity.ClientUri)
            {
                StateHasChanged();
            }
        }

        private static Entity.ClientUri CreateRedirectUri()
            => new();
    }
}
