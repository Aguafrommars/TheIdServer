// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System.Reflection;

namespace Aguacongas.TheIdServer.MAUIApp
{
    public partial class Main
    {
        private readonly List<Assembly> _lazyLoadedAssemblies = new List<Assembly>
        {
            typeof(BlazorApp.Pages.Index).Assembly,
            typeof(BlazorApp.Pages.Api.Api).Assembly,
            typeof(BlazorApp.Pages.Apis.Apis).Assembly,
            typeof(BlazorApp.Pages.ApiScope.ApiScope).Assembly,
            typeof(BlazorApp.Pages.ApiScopes.ApiScopes).Assembly,
            typeof(BlazorApp.Pages.Client.Client).Assembly,
            typeof(BlazorApp.Pages.Clients.Clients).Assembly,
            typeof(BlazorApp.Pages.Culture.Culture).Assembly,
            typeof(BlazorApp.Pages.Cultures.Cultures).Assembly,
            typeof(BlazorApp.Pages.ExternalProvider.ExternalProvider).Assembly,
        };
    }
}
