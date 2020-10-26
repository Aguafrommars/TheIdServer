using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp
{
    public partial class App
    {
        private List<Assembly> _lazyLoadedAssemblies = new List<Assembly>
        {
            typeof(Pages.Index).Assembly
        };

        private Task OnNavigateAsync(NavigationContext args)
        {
            if (args.Path.StartsWith("apis"))
            {
                return LoadAssemblyAsync("Aguacongas.TheIdServer.BlazorApp.Pages.Apis.dll");
            }

            if (args.Path.StartsWith("protectresource"))
            {
                return LoadAssemblyAsync("Aguacongas.TheIdServer.BlazorApp.Pages.Api.dll");
            }
            return Task.CompletedTask;
        }

        private async Task LoadAssemblyAsync(string assemblyName)
        {
            var assemblies = await _assemblyLoader.LoadAssembliesAsync(
                new[] { assemblyName }).ConfigureAwait(false);
            _lazyLoadedAssemblies.AddRange(assemblies.Where(a => !_lazyLoadedAssemblies.Any(l => l.FullName == a.FullName)));
        }
    }
}
