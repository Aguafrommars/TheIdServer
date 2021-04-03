// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
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
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using RichardSzalay.MockHttp;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;

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

                    using var scope = builder.ApplicationServices.CreateScope();
                    var dbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                    if (dbContext != null && !dbContext.Providers.Any(p => p.Scheme == "Google"))
                    {
                        dbContext.Providers.Add(new SchemeDefinition
                        {
                            Scheme = "Google",
                            DisplayName = "Google",
                            SerializedHandlerType = "{\"Name\":\"Microsoft.AspNetCore.Authentication.Google.GoogleHandler\"}",
                            SerializedOptions = "{\"ClientId\":\"818322595124-h0nd8080luc71ba2i19a5kigackfm8me.apps.googleusercontent.com\",\"ClientSecret\":\"ac_tx-O9XvZGNRi4HYfPerx2\",\"AuthorizationEndpoint\":\"https://accounts.google.com/o/oauth2/v2/auth\",\"TokenEndpoint\":\"https://oauth2.googleapis.com/token\",\"UserInformationEndpoint\":\"https://www.googleapis.com/oauth2/v2/userinfo\",\"Events\":{},\"ClaimActions\":[{\"JsonKey\":\"id\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"name\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"given_name\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"family_name\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"link\",\"ClaimType\":\"urn:google:profile\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"email\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"}],\"Scope\":[\"openid\",\"profile\",\"email\"],\"BackchannelTimeout\":\"00:01:00\",\"Backchannel\":{\"DefaultRequestHeaders\":[{\"Key\":\"User-Agent\",\"Value\":[\"Microsoft\",\"ASP.NET\",\"Core\",\"OAuth\",\"handler\"]}],\"DefaultRequestVersion\":\"1.1\",\"Timeout\":\"00:01:00\",\"MaxResponseContentBufferSize\":10485760},\"CallbackPath\":\"/signin-Google\",\"ReturnUrlParameter\":\"ReturnUrl\",\"SignInScheme\":\"Identity.External\",\"RemoteAuthenticationTimeout\":\"00:15:00\",\"CorrelationCookie\":{\"Name\":\".AspNetCore.Correlation.\",\"HttpOnly\":true,\"IsEssential\":true}}"
                        });
                        dbContext.SaveChanges();
                    }
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
            out MockHttpMessageHandler mockHttp,
            bool useJsRuntime = false)
        {
            CreateTestHost(userName, claims, url, sut, testOutputHelper, out host, out mockHttp, useJsRuntime);
            component = host.AddComponent<blazorApp.App>();
        }

        public static void CreateTestHost(string userName,
            IEnumerable<Claim> claims,
            string url,
            TestServer sut,
            ITestOutputHelper testOutputHelper,
            out TestHost host,
            out MockHttpMessageHandler mockHttp,
            bool useJsRuntime = false)
        {
            var navigationInterceptionMock = new Mock<INavigationInterception>();
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
                    .AddSingleton(navigationInterceptionMock.Object)
                    .AddSingleton(navigationInterceptionMock)
                    .AddSingleton(p => new Settings
                    {
                        ApiBaseUrl = appConfiguration["ApiBaseUrl"]
                    })
                    .AddSingleton(localizerMock.Object)
                    .AddSingleton(localizerMock)
                    .AddTransient(p => new HttpClient(sut.CreateHandler()))
                    .AddSingleton<SignOutSessionStateManager, FakeSignOutSessionStateManager>()
                    .AddSingleton<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>()
                    .AddSingleton<IAccessTokenProvider>(p => p.GetRequiredService<FakeAuthenticationStateProvider>())
                    .AddSingleton<AuthenticationStateProvider>(p => p.GetRequiredService<FakeAuthenticationStateProvider>())
                    .AddSingleton(p => new FakeAuthenticationStateProvider(
                        userName,
                        claims))
                    .AddScoped<LazyAssemblyLoader>()
                    .AddHttpClient("oidc")
                    .ConfigureHttpClient(httpClient =>
                    {
                        var apiUri = new Uri(httpClient.BaseAddress, "api");
                        httpClient.BaseAddress = apiUri;
                    })
                    .AddHttpMessageHandler(() => new FakeDelegatingHandler(sut.CreateHandler()));

                if (useJsRuntime)
                {
                    services.AddSingleton(new JSRuntimeImpl())
                        .AddSingleton<IJSRuntime>(p => p.GetRequiredService<JSRuntimeImpl>());
                }
                else
                {
                    var jsRuntimeMock = new Mock<IJSRuntime>();
                    services.AddSingleton(jsRuntimeMock)
                        .AddSingleton(jsRuntimeMock.Object);
                }
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

            public FakeAuthenticationStateProvider(string userName, IEnumerable<Claim> claims)
            {
                if (claims != null && !claims.Any(c => c.Type == "name"))
                {
                    var list = claims.ToList();
                    list.Add(new Claim("name", userName));
                    claims = list;
                }
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

        class FakeDelegatingHandler : DelegatingHandler
        {
            private readonly HttpMessageHandler _handler;

            public FakeDelegatingHandler(HttpMessageHandler handler)
            {
                _handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var method = typeof(HttpMessageHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance);

                if (request.Content is MultipartFormDataContent dataContent)
                {
                    var content = new MultipartFormDataContent();
                    var fileContent = dataContent.First() as StreamContent;
                    var contentDisposition = fileContent.Headers.GetValues("Content-Disposition");
                    var fileName = contentDisposition.First().Split("; ").First(s => s.StartsWith("filename")).Split("=")[1];
                    var file = File.OpenRead(fileName);
                    content.Add(new StreamContent(file), "files", file.Name);
                    request.Content = content;

                }
                return method.Invoke(_handler, new object[] { request, cancellationToken }) as Task<HttpResponseMessage>;
            }
        }
    }

    class JSRuntimeImpl : JSRuntime
    {
        protected override void BeginInvokeJS(long taskId, string identifier, string argsJson)
        {
        }

        protected override void BeginInvokeJS(long taskId, string identifier, string argsJson, JSCallResultType resultType, long targetInstanceId)
        {
        }

        protected override void EndInvokeDotNet(DotNetInvocationInfo invocationInfo, in DotNetInvocationResult invocationResult)
        {
        }
    }

}
