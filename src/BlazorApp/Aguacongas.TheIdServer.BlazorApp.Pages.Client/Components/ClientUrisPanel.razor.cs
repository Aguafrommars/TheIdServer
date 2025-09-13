// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientUrisPanel
    {
        string WsFedRedirectUri
        {
            get => Model.RedirectUris.FirstOrDefault()?.Uri;
            set
            {
                var uri = Model.RedirectUris.FirstOrDefault();
                if (uri is null)
                {
                    uri = new Entity.ClientUri
                    {
                        Kind = Entity.UriKinds.Redirect,
                        Uri = value
                    };
                    Model.RedirectUris.Add(uri);
                    HandleModificationState.EntityCreated(uri);
                    return;
                }

                uri.Uri = value;
                HandleModificationState.EntityUpdated(uri);
            }
        }

        [Parameter]
        public Entity.Client Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected override void OnInitialized()
        {
            HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
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
