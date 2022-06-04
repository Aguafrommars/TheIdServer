// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.AddTheIdServerApp();
var configuration = builder.Configuration;
var settings = configuration.Get<Settings>();
if (settings?.Prerendered == false)
{
    builder.RootComponents.Add<App>("app");
}
            
var host = builder.Build();
var runtime = host.Services.GetRequiredService<IJSRuntime>();
var cultureName = await runtime.InvokeAsync<string>("localStorage.getItem", "culture").ConfigureAwait(false);
if (!string.IsNullOrEmpty(cultureName))
{
    CultureInfo.CurrentCulture = CultureInfo.GetCultures(CultureTypes.AllCultures)
        .FirstOrDefault(c => c.Name == cultureName) ?? CultureInfo.CurrentCulture;
}
await host.RunAsync().ConfigureAwait(false);
