// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.Components
{
    public partial class OAuthOptionsProperties
    {
        private ExternalProviderWrapper<OAuthOptions> _wrapper;
        protected IExternalProvider<OAuthOptions> Model => _wrapper;

        [CascadingParameter]
        public Models.ExternalProvider ModelBase { get; set; }

        protected override void OnInitialized()
        {
            _wrapper = new ExternalProviderWrapper<OAuthOptions>(ModelBase);
            _wrapper.Options ??= _wrapper.DefaultOptions;
            base.OnInitialized();
        }

    }
}
