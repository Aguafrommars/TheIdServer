using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using RichardSzalay.MockHttp;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit.Abstractions;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public static class TestUtils
    {

        public static TestServer CreateTestServer(
                    Action<IServiceCollection> configureServices = null,
                    IEnumerable<KeyValuePair<string, string>> configurationOverrides = null)
        {
            Startup startup = null;
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration))
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\src\Aguacongas.TheIdServer\appsettings.json"));
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    if (configurationOverrides != null)
                    {
                        builder.AddInMemoryCollection(configurationOverrides);
                    }
                })
                .ConfigureServices((context, services) =>
                {
                    startup = new Startup(context.Configuration, context.HostingEnvironment);
                    configureServices?.Invoke(services);
                    startup.ConfigureServices(services);
                    services.AddSingleton<TestUserService>()
                        .AddMvc().AddApplicationPart(startup.GetType().Assembly);
                    configureServices?.Invoke(services);
                })
                .Configure(builder =>
                {
                    builder.Use(async (context, next) =>
                    {
                        var testService = context.RequestServices.GetRequiredService<TestUserService>();
                        context.User = testService.User;
                        await next();
                    });
                    startup.Configure(builder);
                });

            var testServer = new TestServer(webHostBuilder);

            return testServer;
        }

        public static void CreateTestHost(string userName,
            IEnumerable<Claim> claims,
            string url,
            TestServer sut,
            ITestOutputHelper testOutputHelper,
            out TestHost host,
            out RenderedComponent<blazorApp.App> component,
            out MockHttpMessageHandler mockHttp)
        {
            CreateTestHost(userName, claims, url, sut, testOutputHelper, out host, out mockHttp);
            component = host.AddComponent<blazorApp.App>();
        }

        public static void CreateTestHost(string userName, IEnumerable<Claim> claims, string url, TestServer sut, ITestOutputHelper testOutputHelper, out TestHost host, out MockHttpMessageHandler mockHttp)
        {
            var navigationInterceptionMock = new Mock<INavigationInterception>();
            var jsRuntimeMock = new Mock<IJSRuntime>();
            host = new TestHost();
            var httpMock = host.AddMockHttp();
            mockHttp = httpMock;
            var localizerMock = new Mock<ISharedStringLocalizerAsync>();
            localizerMock.Setup(m => m[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            localizerMock.Setup(m => m[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] p) => new LocalizedString(key, string.Format(key, p)));

            host.ConfigureServices(services =>
            {
                var httpClient = sut.CreateClient();
                var appConfiguration = CreateApplicationConfiguration(httpClient);

                WebAssemblyHostBuilderExtensions.ConfigureServices(services, appConfiguration, appConfiguration.Get<Settings>(), httpClient.BaseAddress.ToString());

                sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, claims.Select(c => new Claim(c.Type, c.Value)));
                
                services
                    .AddLogging(configure =>
                    {
                        configure.AddProvider(new TestLoggerProvider(testOutputHelper));
                    })
                    .AddTransient(p => sut.CreateHandler())
                    .AddIdentityServer4AdminHttpStores(p =>
                    {
                        var client = new HttpClient(new BaseAddressAuthorizationMessageHandler(p.GetRequiredService<IAccessTokenProvider>(),
                            p.GetRequiredService<NavigationManager>())
                        {
                            InnerHandler = sut.CreateHandler()
                        })
                        {
                            BaseAddress = new Uri(httpClient.BaseAddress, "api")
                        };
                        return Task.FromResult(client);
                    })
                    .AddSingleton(p => new TestNavigationManager(uri: url))
                    .AddSingleton<NavigationManager>(p => p.GetRequiredService<TestNavigationManager>())
                    .AddSingleton(p => navigationInterceptionMock.Object)
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton<Settings>()
                    .AddSingleton(localizerMock.Object)
                    .AddSingleton<SignOutSessionStateManager, FakeSignOutSessionStateManager>()
                    .AddSingleton<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>()
                    .AddSingleton<IAccessTokenProvider>(p => p.GetRequiredService<FakeAuthenticationStateProvider>())
                    .AddSingleton<AuthenticationStateProvider>(p => p.GetRequiredService<FakeAuthenticationStateProvider>())
                    .AddSingleton(p => new FakeAuthenticationStateProvider(p.GetRequiredService<NavigationManager>(),
                        userName,
                        claims));
            });
        }

        public static IConfigurationRoot CreateApplicationConfiguration(HttpClient httpClient)
        {
            var appConfigurationDictionary = new Dictionary<string, string>
            {
                ["AdministratorEmail"] = "aguacongas@gmail.com",
                ["ApiBaseUrl"] = new Uri(httpClient.BaseAddress, "api").ToString(),
                ["ProviderOptions:Authority"] = httpClient.BaseAddress.ToString(),
                ["ProviderOptions:ClientId"] = "theidserveradmin",
                ["ProviderOptions:DefaultScopes[0]"] = "openid",
                ["ProviderOptions:DefaultScopes[1]"] = "profile",
                ["ProviderOptions:DefaultScopes[2]"] = "theidserveradminapi",
                ["ProviderOptions:PostLogoutRedirectUri"] = new Uri(httpClient.BaseAddress, "authentication/logout-callback").ToString(),
                ["ProviderOptions:RedirectUri"] = new Uri(httpClient.BaseAddress, "authentication/login-callback").ToString(),
                ["ProviderOptions:ResponseType"] = "code",
                ["WelcomeContenUrl"] = "/welcome-fragment.html"
            };
            var appConfiguration = new ConfigurationBuilder().AddInMemoryCollection(appConfigurationDictionary).Build();
            return appConfiguration;
        }

        public class FakeAuthenticationStateProvider : AuthenticationStateProvider, IAccessTokenProvider
        {
            private readonly AuthenticationState _state;
            private readonly NavigationManager _navigationManager;

            public FakeAuthenticationStateProvider(NavigationManager navigationManager, string userName, IEnumerable<Claim> claims)
            {
                if (claims != null && !claims.Any(c => c.Type == "name"))
                {
                    var list = claims.ToList();
                    list.Add(new Claim("name", userName));
                    claims = list;
                }
                _navigationManager = navigationManager;
                _state = new AuthenticationState(new FakeClaimsPrincipal(new FakeIdendity(userName, claims)));
            }

            public override Task<AuthenticationState> GetAuthenticationStateAsync()
            {
                return Task.FromResult(_state);
            }

            public ValueTask<AccessTokenResult> RequestAccessToken()
            {
                return new ValueTask<AccessTokenResult>(new AccessTokenResult(AccessTokenResultStatus.Success,
                    new AccessToken
                    {
                        Expires = DateTimeOffset.Now.AddDays(1),
                        GrantedScopes = new string[] { "openid", "profile", "theidseveradminaoi" },
                        Value = "test"
                    },
                    "http://exemple.com"));
            }

            public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
            {
                throw new NotImplementedException();
            }
        }

        public class AccessTokenProviderAccessor : IAccessTokenProviderAccessor
        {
            public AccessTokenProviderAccessor(IAccessTokenProvider accessTokenProvider)
            {
                TokenProvider = accessTokenProvider;
            }

            public IAccessTokenProvider TokenProvider { get; }
        }

        public class FakeClaimsPrincipal : ClaimsPrincipal
        {
            public FakeClaimsPrincipal(FakeIdendity idendity) : base(idendity)
            {

            }

            public override bool IsInRole(string role)
            {
                return Identity.IsAuthenticated && Claims != null && Claims.Any(c => c.Type == "role" && c.Value == role);
            }
        }

        public class FakeIdendity : ClaimsIdentity
        {
            private readonly string _userName;
            private bool _IsAuthenticated = true;

            public FakeIdendity(string userName, IEnumerable<Claim> claims) : base(claims)
            {
                _userName = userName;
            }
            public override string AuthenticationType => "Bearer";

            public override bool IsAuthenticated => _IsAuthenticated;

            public void SetIsAuthenticated(bool value)
            {
                _IsAuthenticated = value;
            }

            public override string Name => _userName;
        }

        class FakeSignOutSessionStateManager : SignOutSessionStateManager
        {
            public FakeSignOutSessionStateManager(IJSRuntime jsRuntime) : base(jsRuntime)
            { }

            public override ValueTask SetSignOutState()
            {
                return new ValueTask();
            }

            public override Task<bool> ValidateSignOutState()
            {
                return Task.FromResult(true);
            }
        }
    }

}
