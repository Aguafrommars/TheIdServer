using Aguacongas.IdentityServer.Store.Entity;
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
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using System.Reflection;
using System.Security.Cryptography;
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
            .AddTransient<IClientAssertionService, ClientAssertionService>()
            .AddSingleton<AssertionService>()
            .AddAdminApplication(new Settings
            {
                ApiBaseUrl = baseAdress,
            })
            .AddRazorPages();

var jwk = CreateDPoPJsonWebKey();
services.AddBff(options =>
    {
        options.DPoPJsonWebKey = jwk;
    })
    .ConfigureOpenIdConnect(options =>
    {
        options.Scope.Clear();
        configuration.Bind("ProviderOptions", options);
        options.ClaimActions.MapAll();
        options.BackchannelHttpHandler = new AssertionInjectionHandler(new AssertionService(configuration))
        {
            InnerHandler = options.BackchannelHttpHandler ?? new HttpClientHandler()
        };
    })
    .ConfigureCookies(options =>
    {
        options.Cookie.Name = "__Host-blazor";
        options.Cookie.SameSite = SameSiteMode.Strict;
    })
    .AddServerSideSessions()
    .AddRemoteApis();

services.AddOpenIdConnectAccessTokenManagement(options =>
    {
        options.DPoPJsonWebKey = jwk;
    });

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
        var entitiesType = GetEntityTypes();
        foreach (var entityType in entitiesType)
        {
            var endpoint = $"/{entityType.Name.ToLower()}";
            endpoints.MapRemoteBffApiEndpoint(endpoint, new Uri($"{baseAdress}{endpoint}"))
                .WithAccessToken(RequiredTokenType.UserOrNone);
        }
    });

await app.RunAsync().ConfigureAwait(false);

static IEnumerable<Type> GetEntityTypes()
{
    var assembly = typeof(IEntityId).GetTypeInfo().Assembly;
    var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
        !t.IsAbstract &&
        t.GetInterface("IEntityId") != null);
    return entityTypeList;
}

static DPoPProofKey CreateDPoPJsonWebKey()
{
    var rsaKey = new RsaSecurityKey(RSA.Create(2048));
    var jwk = JsonWebKeyConverter.ConvertFromSecurityKey(rsaKey);
    jwk.Alg = "PS256";
    return DPoPProofKey.Parse(JsonSerializer.Serialize(jwk));
}