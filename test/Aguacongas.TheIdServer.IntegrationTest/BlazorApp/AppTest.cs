using Aguacongas.TheIdServer.Blazor.Oidc;
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
using Microsoft.AspNetCore.ResponseCompression;
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
            var server = CreateTestServer(connection, connectionApp);

            using var scope = server.Host.Services.CreateScope();
            SeedData.SeedUsers(scope);
            SeedData.SeedConfiguration(scope);

            var host = new TestHost();
            var options = new Blazor.Oidc.AuthorizationOptions();
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var navigationInterceptionMock = new Mock<INavigationInterception>();
            var navigationManager = new TestNavigationManager(uri: "http://exemple.com");

            var httpClient = server.CreateClient();
            options.Authority = httpClient.BaseAddress.ToString();

            host.ConfigureServices(services =>
            {
                new blazorApp.Startup().ConfigureServices(services);
                services
                    .AddLogging(configure =>
                    {
                        configure.AddProvider(testLoggerProvider);
                    })
                    .AddIdentityServer4HttpStores(p => Task.FromResult(httpClient))
                    .AddSingleton(p => navigationManager)
                    .AddSingleton<NavigationManager>(p => p.GetRequiredService<TestNavigationManager>())
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            var waitHandle = new ManualResetEvent(false);
            httpMock.Fallback.Respond(httpClient);
            jsRuntimeMock.Setup(m => m.InvokeAsync<object>("sessionStorage.setItem", It.IsAny<object[]>()))
                .ReturnsAsync(null);

            string redirectUri = null;
            navigationManager.OnNavigateToCore = (uri, f) =>
            {
                redirectUri = uri;
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
                    options.Authority,
                    ClientId = "theidserveradmin",
                    Scope = "openid profile theidserveradminapi"
                });
            });

            host.WaitForContains(component, "You are redirecting to the login page. please wait");

            WaitForHttpResponse(waitHandle);

            using var authorizeResponse = await httpClient.GetAsync(redirectUri);
            Assert.Equal(HttpStatusCode.Redirect, authorizeResponse.StatusCode);


            using var loginPageResponse = await httpClient.GetAsync(authorizeResponse.Headers.Location);
            var loginPageContent = await loginPageResponse.Content.ReadAsStringAsync();

            var document = new HtmlDocument();
            document.LoadHtml(loginPageContent);
            var form = document.DocumentNode.SelectSingleNode("//body//form");
            Assert.NotNull(form);
            var inputs = form.ParentNode.SelectNodes("input");
            Assert.NotEmpty(inputs);

            var redirectUrl = inputs.FirstOrDefault(n => n.Attributes.Any(a => a.Value == "ReturnUrl"))?
                .Attributes?
                .FirstOrDefault(a => a.Name == "value")?
                .Value;

            Assert.NotNull(redirectUri);
            var validationToken = inputs.FirstOrDefault(n => n.Attributes.Any(a => a.Value == "__RequestVerificationToken"))?
                .Attributes?
                .FirstOrDefault(a => a.Name == "value")?
                .Value;

            Assert.NotNull(validationToken);

            using var postContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Username"] = "alice",
                ["Password"] = "Pass123$",
                ["ReturnUrl"] = redirectUrl,
                ["__RequestVerificationToken"] = validationToken,
                ["RememberLogin"] = "false",
                ["button"] = "login"
            });
            var cookie = loginPageResponse.Headers.First(h => h.Key == "Set-Cookie").Value.First().Split(';').First();
            postContent.Headers.Add("Cookie", cookie);

            using var postLoginResponse = await httpClient.PostAsync(authorizeResponse.Headers.Location, postContent);
            var postLoginContent = await postLoginResponse.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Redirect, postLoginResponse.StatusCode);

            using var redirectLoginResponse = await httpClient.GetAsync(postLoginResponse.Headers.Location);
            var redirectLoginContent = await redirectLoginResponse.Content.ReadAsStringAsync();
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
                    .AddIdentityServer4EntityFrameworkStores<ApplicationUser, ApplicationDbContext>(options =>
                        options.UseSqlite(connection));

                    services.AddIdentity<ApplicationUser, IdentityRole>(
                            options => options.SignIn.RequireConfirmedAccount = true)
                        .AddEntityFrameworkStores<ApplicationDbContext>()
                        .AddDefaultTokenProviders();

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

                    app.UseStaticFiles()
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
