using Aguacongas.TheIdServer.BlazorApp;
using Aguacongas.TheIdServer.BlazorApp.BFF.Models;
using Aguacongas.TheIdServer.BlazorApp.BFF.Models.Models;
using Aguacongas.TheIdServer.BlazorApp.BFF.Services;
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Duende.AccessTokenManagement;
using Duende.AccessTokenManagement.OpenIdConnect;
using Duende.Bff;
using Duende.Bff.AccessTokenManagement;
using Duende.Bff.Blazor;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
services.Configure<SiteOptions>(configuration.GetSection(nameof(SiteOptions)))
    .Configure<OidcProviderOptions>(options => configuration.Bind("ProviderOptions", options))
    .AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
services.AddRemoteAuthentication<RemoteAuthenticationState, RemoteUserAccount, OidcProviderOptions>();
var apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl") ?? throw new InvalidOperationException("API base URL is not configured");
var baseAdress = apiBaseUrl.EndsWith('/') ? $"{apiBaseUrl}api" : $"{apiBaseUrl}/api";

services.AddAdminHttpStores(p => Task.FromResult(p.GetRequiredService<IHttpClientFactory>().CreateClient("AdminApi")))
    .AddHttpClient("AdminApi")
    .ConfigureHttpClient(httpClient =>
    {
        httpClient.BaseAddress = new Uri(baseAdress);
    })
    .AddDefaultAccessTokenResiliency()
    .AddUserAccessTokenHandler();

services.Configure<HostModelOptions>(configuration.GetSection(nameof(HostModelOptions)))
            .AddScoped<ThemeService>()
            .AddScoped<LazyAssemblyLoader>()
            .AddScoped<AuthenticationStateProvider, RemoteAuthenticationService>()
            .AddScoped<NavigationManager, PreRenderNavigationManager>()
            .AddScoped<ISharedStringLocalizerAsync, StringLocalizer>()
            .AddTransient<IReadOnlyCultureStore, PreRenderCultureStore>()
            .AddTransient<IReadOnlyLocalizedResourceStore, PreRenderLocalizedResourceStore>()
            .AddTransient<IAccessTokenProvider, AccessTokenProvider>()
            .AddTransient<IJSRuntime, Aguacongas.TheIdServer.BlazorApp.BFF.Services.JSRuntime>()
            .AddTransient<IClaimsTransformation, CustomClaimsTransformer>()
            .AddAdminApplication(new Settings
            {
                ApiBaseUrl = baseAdress,
            })
            .AddRazorPages();

services.AddBff()
        .ConfigureOpenIdConnect(options =>
        {
            options.ResponseMode = "query";
            options.Scope.Clear();
            configuration.Bind("ProviderOptions", options);
            options.MapInboundClaims = false;
            options.ClaimActions.MapAll();
            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;

            options.TokenValidationParameters.NameClaimType = "name";
            options.TokenValidationParameters.RoleClaimType = "role";
        })
        .ConfigureCookies(options =>
        {
            options.Cookie.Name = "__Host-blazor";
            options.Cookie.SameSite = SameSiteMode.Strict;
        })
        .AddServerSideSessions()
        .AddBlazorServer()
        .AddRemoteApis();

services.AddCascadingAuthenticationState()
    .AddAuthorization(options =>
        options.AddIdentityServerPolicies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
app.UseStaticFiles()
    .UseBlazorFrameworkFiles()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .Use(async (context, next) =>
    {
        if (!context.Request.Path.Equals("/bff/user", StringComparison.OrdinalIgnoreCase))
        {
            await next().ConfigureAwait(false);
            return;
        }

        // Intercept the response from the /bff/user endpoint to fix the claims that are returned as JSON arrays instead of strings.

        var response = context.Response;
        var originalBodyStream = response.Body;
        using var memoryStream = new MemoryStream();
        response.Body = memoryStream;

        await next().ConfigureAwait(false);

        context.Response.Body = originalBodyStream;
        if (memoryStream.Length == 0)
        {
            return;
        }
        memoryStream.Position = 0;
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<ClaimRecord>>(memoryStream, cancellationToken: context.RequestAborted).ConfigureAwait(false);
        // Find claims that have values that are JSON arrays and fix them by deserializing the array and creating individual claims for each value.
        var claimsToFixeList = result!.Where(c => c.Value?.ToString()?.StartsWith('[') == true);
        var fixedList = result!.Where(c => !claimsToFixeList.Contains(c));
        foreach (var claim in claimsToFixeList)
        {
            var values = JsonSerializer.Deserialize<IEnumerable<string>>(claim.Value.ToString()!);
            fixedList = fixedList.Concat(values!.Select(v => new ClaimRecord(claim.Type, v)
            {
                ValueType = claim.ValueType
            }));
        }
        var modifiedBody = JsonSerializer.Serialize(fixedList);

        var bytes = Encoding.UTF8.GetBytes(modifiedBody);
        response.Body = originalBodyStream;
        response.ContentLength = bytes.Length;
        await response.Body.WriteAsync(bytes, context.RequestAborted);
    })
    .UseBff()
    .UseAuthorization()
    .UseAntiforgery()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapFallbackToPage("/_Host");
        endpoints.MapStaticAssets();
        endpoints.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode();
        endpoints.MapRemoteBffApiEndpoint("/api", new Uri(baseAdress))
            .WithAccessToken(RequiredTokenType.UserOrNone);
    });

await app.RunAsync().ConfigureAwait(false);
