// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientUri
    {
        [Parameter]
        public Entity.ClientUri Model { get; set; }

        [Parameter]
        public bool IsSpaClient { get; set; }

        [Parameter]
        public bool CanHandlePostLogout { get; set; }

        [Parameter]
        public bool IsSaml2PClient { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }
        bool Cors
        {
            get { return (Model.Kind & Entity.UriKinds.Cors) == Entity.UriKinds.Cors; }
            set
            {
                if (value)
                {
                    Model.Kind |= Entity.UriKinds.Cors;
                }
                else
                {
                    Model.Kind &= ~Entity.UriKinds.Cors;
                }
                HandleModificationState.EntityUpdated(Model);
            }
        }

        bool Redirect
        {
            get { return (Model.Kind & Entity.UriKinds.Redirect) == Entity.UriKinds.Redirect; }
            set
            {
                if (value)
                {
                    Model.Kind |= Entity.UriKinds.Redirect;
                }
                else
                {
                    Model.Kind &= ~Entity.UriKinds.Redirect;
                }
                HandleModificationState.EntityUpdated(Model);
            }
        }

        bool Metadata
        {
            get { return (Model.Kind & Entity.UriKinds.Saml2Metadata) == Entity.UriKinds.Saml2Metadata; }
            set
            {
                if (value)
                {
                    Model.Kind |= Entity.UriKinds.Saml2Metadata;
                }
                else
                {
                    Model.Kind &= ~Entity.UriKinds.Saml2Metadata;
                }
                HandleModificationState.EntityUpdated(Model);
            }
        }

        bool PostLogout
        {
            get { return (Model.Kind & Entity.UriKinds.PostLogout) == Entity.UriKinds.PostLogout; }
            set
            {
                if (value)
                {
                    Model.Kind |= Entity.UriKinds.PostLogout;
                }
                else
                {
                    Model.Kind &= ~Entity.UriKinds.PostLogout;
                }
                HandleModificationState.EntityUpdated(Model);
            }
        }
    }
}
