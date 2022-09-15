using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Aguacongas.TheIdServer.Services
{
    public class PreRenderNavigationManager : NavigationManager, IHostEnvironmentNavigationManager
    {
        protected override void NavigateToCore(string uri, NavigationOptions options)
        {
            // don't do anything
        }

        void IHostEnvironmentNavigationManager.Initialize(string baseUri, string uri)
        {
            Initialize(baseUri, uri);
        }
    }
}
