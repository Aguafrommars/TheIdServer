using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.Components
{
    public partial class RemoteOptionsProperties
    {
        private ExternalProviderWrapper<RemoteAuthenticationOptions> _wrapper;
        protected IExternalProvider<RemoteAuthenticationOptions> Model => _wrapper;

        [CascadingParameter]
        public Models.ExternalProvider ModelBase { get; set; }

        protected override void OnInitialized()
        {
            _wrapper = new ExternalProviderWrapper<RemoteAuthenticationOptions>(ModelBase);
            _wrapper.Options ??= _wrapper.DefaultOptions;
            base.OnInitialized();
        }
    }
}
