// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp
{
    [SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "entry point")]
    [SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "<Pending>")]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.AddTheIdServerApp();
            var host = builder.Build();
            var runtime = host.Services.GetRequiredService<IJSRuntime>();
            var cultureName = await runtime.InvokeAsync<string>("localStorage.getItem", "culture").ConfigureAwait(false);
            if (!string.IsNullOrEmpty(cultureName))
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .FirstOrDefault(c => c.Name == cultureName) ?? CultureInfo.CurrentCulture;
            }
            await host.RunAsync().ConfigureAwait(false);
        }
    }
}
