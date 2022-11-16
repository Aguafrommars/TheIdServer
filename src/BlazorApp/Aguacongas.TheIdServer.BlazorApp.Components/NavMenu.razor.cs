// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class NavMenu
    {
        [Inject]
        private IOptions<MenuOptions> Options { get; set; }

        bool collapseNavMenu = true;

        string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }

        private string _gitHash;

        string GitHash
        {
            get
            {
                if (string.IsNullOrEmpty(_gitHash))
                {
                    var version = "1.0.0+LOCALBUILD"; // Dummy version for local dev
                    var appAssembly = Assembly.GetExecutingAssembly();
                    var infoVerAttr = appAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute))
                        .FirstOrDefault() as AssemblyInformationalVersionAttribute;

                    if (infoVerAttr is not null && infoVerAttr.InformationalVersion.Length > 6)
                    {
                        // Hash is embedded in the version after a '+' symbol, e.g. 1.0.0+a34a913742f8845d3da5309b7b17242222d41a21
                        version = infoVerAttr.InformationalVersion;
                    }
                    _gitHash = version[(version.IndexOf('+') + 1)..];
                }

                return _gitHash;
            }
        }
    }
}