// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.Components.Routing;
using System;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class EntityInfo
    {
        private string _entityType;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            _navigationManager.LocationChanged += _navigationManager_LocationChanged;
            GetEntityType();
        }

        private void GetEntityType()
        {
            var uri = new Uri(_navigationManager.Uri);
            var segments = uri.Segments;
            if (segments.Length != 3)
            {
                _entityType = null;
                return;
            }

            var entityTypeName = segments[1].Trim('/')
                .Replace("protectresource", "api")
                .Replace("apiscope", "scope")
                .Replace("identityresource", "identity")
                .Replace("externalprovider", "provider");
            _entityType = Localizer[entityTypeName];
        }

        private void _navigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            GetEntityType();
            InvokeAsync(StateHasChanged);
        }
    }
}
