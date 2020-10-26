using Microsoft.AspNetCore.Components.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp
{
    public partial class App
    {
        private readonly List<Assembly> _lazyLoadedAssemblies = new List<Assembly>
        {
            typeof(Pages.Index).Assembly
        };

        private readonly string[] _pageKindList = new []
        {
            "Api",
            "ApiScope",
            "Client",
            "ExternalProvider",
            "Identities",
            "Identity",
            "Import",
            "Key",
            "Role",
            "User"
        };

        private Task OnNavigateAsync(NavigationContext args)
        {
            var path = args.Path;
            if (path.StartsWith("protectresource"))
            {
                return LoadAssemblyAsync("Aguacongas.TheIdServer.BlazorApp.Pages.Api.dll");
            }

            var pageKind = _pageKindList.FirstOrDefault(k => path.StartsWith($"{k.ToLower()}s"));
            if (pageKind != null)
            {
                return LoadAssemblyAsync($"Aguacongas.TheIdServer.BlazorApp.Pages.{pageKind}s.dll");
            }

            pageKind = _pageKindList.FirstOrDefault(k => path.StartsWith($"{k.ToLower()}"));
            if (pageKind != null)
            {
                return LoadAssemblyAsync($"Aguacongas.TheIdServer.BlazorApp.Pages.{pageKind}.dll");
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
