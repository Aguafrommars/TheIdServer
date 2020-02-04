using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp
{
    public class AppTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AppTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task FullLoginTest()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var connectionApp = new SqliteConnection("DataSource=:memory:");
            connectionApp.Open();
            var testLoggerProvider = new TestLoggerProvider(_testOutputHelper);
            using var server = CreateTestServer(connection, connectionApp);

            using var scope = server.Host.Services.CreateScope();
            var sessionStore = new ConcurrentDictionary<object, object>();
            NavigateToLoginPage(testLoggerProvider, server, scope, sessionStore, out HttpClient httpClient, out string redirectUri);

            using var authorizeResponse = await httpClient.GetAsync(redirectUri);
            Assert.Equal(HttpStatusCode.Redirect, authorizeResponse.StatusCode);

            using var loginPageResponse = await httpClient.GetAsync(authorizeResponse.Headers.Location);
            var loginPageContent = await loginPageResponse.Content.ReadAsStringAsync();

            Assert.NotNull(redirectUri);
            ParseForm(loginPageContent, out string redirectUrl, out string validationToken);

            using var postContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Username"] = "alice",
                ["Password"] = "Pass123$",
                ["ReturnUrl"] = redirectUrl.Replace("&amp;", "&"),
                ["__RequestVerificationToken"] = validationToken,
                ["RememberLogin"] = "true",
                ["button"] = "login"
            });
            var cookie = loginPageResponse.Headers.First(h => h.Key == "Set-Cookie").Value.First().Split(';').First();
            postContent.Headers.Add("Cookie", cookie);

            using var postLoginResponse = await httpClient.PostAsync(authorizeResponse.Headers.Location, postContent);            
            Assert.Equal(HttpStatusCode.Redirect, postLoginResponse.StatusCode);

            var cookies = postLoginResponse.Headers.First(h => h.Key == "Set-Cookie").Value
                .Select(v => v.Split(';').First())
                .ToList();

            cookies.Add(cookie);

            using var message = new HttpRequestMessage(HttpMethod.Get, postLoginResponse.Headers.Location);
            message.Headers.Add("Cookie", string.Join("; ", cookies));

            using var redirectLoginResponse = await httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.Redirect, postLoginResponse.StatusCode);

            using var message2 = new HttpRequestMessage(HttpMethod.Get, redirectLoginResponse.Headers.Location);
            message2.Headers.Add("Cookie", string.Join("; ", cookies));

            using var consentResponse = await httpClient.SendAsync(message2);
            Assert.Equal(HttpStatusCode.OK, consentResponse.StatusCode);

            var consentContent = await consentResponse.Content.ReadAsStringAsync();

            ParseForm(consentContent, out redirectUrl, out validationToken);
            using var postConsentContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ReturnUrl", redirectUrl.Replace("&amp;", "&")),
                new KeyValuePair<string, string>("__RequestVerificationToken", validationToken),
                new KeyValuePair<string, string>("ScopesConsented", "openid"),
                new KeyValuePair<string, string>("ScopesConsented", "profile"),
                new KeyValuePair<string, string>("ScopesConsented", "theidserveradminapi"),
                new KeyValuePair<string, string>("RememberConsent", "true"),
                new KeyValuePair<string, string>("button", "yes"),
            });
            postConsentContent.Headers.Add("Cookie", string.Join("; ", cookies));

            using var postConsentResponse = await httpClient.PostAsync(redirectLoginResponse.Headers.Location, postConsentContent);
            Assert.Equal(HttpStatusCode.OK, postConsentResponse.StatusCode);

            var consentRedirectContent = await postConsentResponse.Content.ReadAsStringAsync();

            var document = new HtmlDocument();
            document.LoadHtml(consentRedirectContent);
            var meta = document.DocumentNode.SelectSingleNode("//body//meta");

            redirectUrl = meta.Attributes.First(a => a.Name == "data-url").Value.Replace("&amp;", "&");
            cookie = postConsentResponse.Headers.First(h => h.Key == "Set-Cookie").Value.First().Split(';').First();
            cookies.Add(cookie);

            using var message3 = new HttpRequestMessage(HttpMethod.Get, $"{redirectUrl}&{Guid.NewGuid()}");
            message3.Headers.Add("Cookie", string.Join("; ", cookies));
            using var consentRedirectResponse = await httpClient.SendAsync(message3);

            OpenLogggedPage(testLoggerProvider, httpClient, sessionStore, consentRedirectResponse.Headers.Location.ToString());
        }

        private static void ParseForm(string html, out string redirectUrl, out string validationToken)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var form = document.DocumentNode.SelectSingleNode("//body//form");
            Assert.NotNull(form);
            var inputs = form.ParentNode.SelectNodes("input");
            Assert.NotEmpty(inputs);

            redirectUrl = inputs.FirstOrDefault(n => n.Attributes.Any(a => a.Value == "ReturnUrl"))?
                .Attributes?
                .FirstOrDefault(a => a.Name == "value")?
                .Value;
            Assert.NotNull(redirectUrl);
            validationToken = inputs.FirstOrDefault(n => n.Attributes.Any(a => a.Value == "__RequestVerificationToken"))?
                .Attributes?
                .FirstOrDefault(a => a.Name == "value")?
                .Value;
            Assert.NotNull(validationToken);
        }

        private static void OpenLogggedPage(TestLoggerProvider testLoggerProvider, HttpClient httpClient, ConcurrentDictionary<object, object> sessionStore, string location)
        {
            using var host = new TestHost();
            var options = new Blazor.Oidc.AuthorizationOptions();
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var navigationInterceptionMock = new Mock<INavigationInterception>();
            var navigationManager = new TestNavigationManager(uri: location);

            options.Authority = httpClient.BaseAddress.ToString();

            host.ConfigureServices(services =>
            {
                new blazorApp.Startup().ConfigureServices(services);
                services
                    .AddLogging(configure =>
                    {
                        configure.AddProvider(testLoggerProvider);
                    })
                    .AddIdentityServer4AdminHttpStores(p => Task.FromResult(httpClient))
                    .AddSingleton(p => navigationManager)
                    .AddSingleton<NavigationManager>(p => p.GetRequiredService<TestNavigationManager>())
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            httpMock.Fallback.Respond(httpClient);

            foreach(var key in sessionStore.Keys)
            {
                jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", It.Is<object[]>(p => p[0].ToString() == key.ToString())))
                    .ReturnsAsync<string, object[], IJSRuntime, string>((_, array) =>
                        {
                            if (sessionStore.TryGetValue(array[0], out object value))
                            {
                                return (string)value;
                            }
                            throw new InvalidOperationException($"sessionStore doesn't contain key {key}");
                        });

            }

            string navigatedUri = null;
            var waitHandle = new ManualResetEvent(false);
            navigationManager.OnNavigateToCore = (uri, f) =>
            {
                navigatedUri = uri;
                waitHandle.Set();
            };

            var settingsRequest = httpMock.Capture("/settings.json");
            var component = host.AddComponent<blazorApp.App>();

            var markup = component.GetMarkup();
            Assert.Contains("Authentication in progress", markup);

            host.WaitForNextRender(() =>
            {
                settingsRequest.SetResult(new
                {
                    ApiBaseUrl = $"{options.Authority}api",
                    Authority = options.Authority.TrimEnd('/'),
                    ClientId = "theidserveradmin",
                    Scope = "openid profile theidserveradminapi"
                });
            });


            WaitForHttpResponse(waitHandle);

            Assert.Equal("http://exemple.com", navigatedUri);
        }

        private static void NavigateToLoginPage(TestLoggerProvider testLoggerProvider, TestServer server, IServiceScope scope, ConcurrentDictionary<object, object> sessionStore, out HttpClient httpClient, out string redirectUri)
        {
            SeedData.SeedUsers(scope);
            SeedData.SeedConfiguration(scope);

            using var host = new TestHost();
            var options = new Blazor.Oidc.AuthorizationOptions();
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var navigationInterceptionMock = new Mock<INavigationInterception>();
            var navigationManager = new TestNavigationManager(uri: "http://exemple.com");

            var client = server.CreateClient();
            httpClient = client;
            options.Authority = httpClient.BaseAddress.ToString();

            host.ConfigureServices(services =>
            {
                new blazorApp.Startup().ConfigureServices(services);
                services
                    .AddLogging(configure =>
                    {
                        configure.AddProvider(testLoggerProvider);
                    })
                    .AddIdentityServer4AdminHttpStores(p => Task.FromResult(client))
                    .AddSingleton(p => navigationManager)
                    .AddSingleton<NavigationManager>(p => p.GetRequiredService<TestNavigationManager>())
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            httpMock.Fallback.Respond(httpClient);
            jsRuntimeMock.Setup(m => m.InvokeAsync<object>("sessionStorage.setItem", It.IsAny<object[]>()))
                .Callback<string, object[]>((_, array) => sessionStore.AddOrUpdate(array[0],  array[1], (k,v) => array[1]))
                .ReturnsAsync(null);

            string navigatedUri = null;
            var waitHandle = new ManualResetEvent(false);
            navigationManager.OnNavigateToCore = (uri, f) =>
            {
                navigatedUri = uri;
                waitHandle.Set();
            };

            var settingsRequest = httpMock.Capture("/settings.json");
            var component = host.AddComponent<blazorApp.App>();

            var markup = component.GetMarkup();
            Assert.Contains("Authentication in progress", markup);

            host.WaitForNextRender(() =>
            {
                settingsRequest.SetResult(new
                {
                    ApiBaseUrl = $"{options.Authority}api",
                    Authority = options.Authority.TrimEnd('/'),
                    ClientId = "theidserveradmin",
                    Scope = "openid profile theidserveradminapi"
                });
            });

            host.WaitForContains(component, "You are redirecting to the login page. please wait");

            WaitForHttpResponse(waitHandle);

            redirectUri = navigatedUri;
        }

        private static void WaitForHttpResponse(ManualResetEvent waitHandle)
        {
            if (Debugger.IsAttached)
            {
                waitHandle.WaitOne();
            }
            else
            {
                waitHandle.WaitOne(5000);
            }
        }

        private static TestServer CreateTestServer(SqliteConnection connection, SqliteConnection connectionApp)
        {
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration))
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\src\Aguacongas.TheIdServer\appsettings.json"));
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite(connectionApp))
                    .AddIdentityServer4AdminEntityFrameworkStores<ApplicationUser, ApplicationDbContext>(options =>
                        options.UseSqlite(connection));

                    services.AddIdentity<ApplicationUser, IdentityRole>(
                            options => options.SignIn.RequireConfirmedAccount = false)
                        .AddEntityFrameworkStores<ApplicationDbContext>()
                        .AddDefaultTokenProviders();

                    services.AddRazorPages(options =>
                    {
                        options.Conventions.AuthorizeAreaFolder("Identity", "/Account");
                    });

                    services.AddControllersWithViews(options =>
                    {
                        options.AddIdentityServerAdminFilters();
                    })
                        .AddNewtonsoftJson(options =>
                        {
                            var settings = options.SerializerSettings;
                            settings.NullValueHandling = NullValueHandling.Ignore;
                            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        })
                        .AddIdentityServerAdmin();

                    services.Configure<IISOptions>(iis =>
                    {
                        iis.AuthenticationDisplayName = "Windows";
                        iis.AutomaticAuthentication = false;
                    });

                    services.AddIdentityServer(options =>
                    {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseSuccessEvents = true;
                    })
                        .AddAspNetIdentity<ApplicationUser>()
                        .AddDefaultSecretParsers()
                        .AddDefaultSecretValidators()
                        .AddDeveloperSigningCredential();

                    services.AddAuthorization(options =>
                    {
                        options.AddIdentityServerPolicies();
                    })
                        .AddAuthentication()
                        .AddIdentityServerAuthentication("Bearer", options =>
                        {
                            options.Authority = "https://localhost:5443";
                            options.RequireHttpsMetadata = false;
                            options.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Both;
                            options.ApiName = "theidserveradminapi";
                            options.EnableCaching = true;
                            options.CacheDuration = TimeSpan.FromMinutes(10);
                            options.LegacyAudienceValidation = true;
                        });

                    services.AddRazorPages(options =>
                        {
                            options.Conventions.AuthorizeAreaFolder("Identity", "/Account");
                        });

                })
                .Configure(app =>
                {
                    app.UseDeveloperExceptionPage()
                        .UseDatabaseErrorPage();

                    app.UseSerilogRequestLogging()
                        .UseStaticFiles()
                        .UseIdentityServer()
                        .UseRouting()
                        .UseAuthentication()
                        .UseAuthorization()
                        .UseEndpoints(endpoints =>
                        {
                            endpoints.MapDefaultControllerRoute();
                            endpoints.MapRazorPages();
                        });

                });

            var testServer = new TestServer(webHostBuilder);

            return testServer;
        }
    }
}
