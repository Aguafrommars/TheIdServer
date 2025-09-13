// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.DynamicConfiguration.Razor.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.Models;
using Aguacongas.TheIdServer.UI;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp
{
    public class TheIdServerFactory : WebApplicationFactory<AccountController>
    {
        public Task DbActionAsync<T>(Func<T, Task> action) where T : DbContext
        {
            using var scope = Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<T>();
            return action(context);
        }

        public void ConfigureTestContext(string userName,
            IEnumerable<Claim> claims,
            TestContext testContext)
        {
            testContext.JSInterop.Mode = JSRuntimeMode.Loose;
            testContext.Services.AddScoped<JSRuntimeImpl>();

            var authContext = testContext.AddTestAuthorization();
            authContext.SetAuthorized(userName);
            authContext.SetClaims(claims.ToArray());
            authContext.SetPolicies(claims.Where(c => c.Type == "role").Select(c => c.Value).ToArray());

            var localizerMock = new Mock<ISharedStringLocalizerAsync>();
            localizerMock.Setup(m => m[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            localizerMock.Setup(m => m[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] p) => new LocalizedString(key, string.Format(key, p)));

            var services = testContext.Services;
            var httpClient = CreateClient();
            var appConfiguration = TestUtils.CreateApplicationConfiguration(httpClient);

            WebAssemblyHostBuilderExtensions.ConfigureServices(services, appConfiguration, appConfiguration.Get<Settings>());           

            Services.GetRequiredService<TestUserService>()
                .SetTestUser(true, claims.Select(c => new Claim(c.Type, c.Value)));

            services.AddTransient(p => Server.CreateHandler())
                .AddAdminHttpStores(p =>
                {
                    var client = new HttpClient(new BaseAddressAuthorizationMessageHandler(p.GetRequiredService<IAccessTokenProvider>(),
                        p.GetRequiredService<NavigationManager>())
                    {
                        InnerHandler = Server.CreateHandler()
                    })
                    {
                        BaseAddress = new Uri(httpClient.BaseAddress ?? new Uri(string.Empty), "api")
                    };
                    return Task.FromResult(client);
                })
                .AddScoped(p => new Settings
                {
                    ApiBaseUrl = appConfiguration["ApiBaseUrl"],
                    WelcomeContenUrl = $"{httpClient.BaseAddress}api/welcomefragment"
                })
                .AddScoped(p => localizerMock.Object)
                .AddScoped(p => localizerMock)
                .AddTransient(p => new HttpClient(Server.CreateHandler()))
                .AddTransient<BaseAddressAuthorizationMessageHandler>()
                .AddSingleton<IAccessTokenProviderAccessor, TestUtils.AccessTokenProviderAccessor>()
                .AddScoped<IAccessTokenProvider>(p => p.GetRequiredService<TestUtils.FakeAuthenticationStateProvider>())
                .AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<TestUtils.FakeAuthenticationStateProvider>())
                .AddScoped(p => new TestUtils.FakeAuthenticationStateProvider(
                    userName,
                    claims))
                .AddScoped<LazyAssemblyLoader>()
                .AddHttpClient("oidc")
                .ConfigureHttpClient(httpClient =>
                {
                    var apiUri = new Uri(httpClient.BaseAddress ?? new Uri(string.Empty), "api");
                    httpClient.BaseAddress = apiUri;
                })
                .AddHttpMessageHandler(() => new TestUtils.FakeDelegatingHandler(Server.CreateHandler()));

            services.AddHttpClient(nameof(ConfigurationService))
                .AddHttpMessageHandler(() => new TestUtils.FakeDelegatingHandler(Server.CreateHandler()));
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(webBuilder =>
                {
                    webBuilder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.json"));
                    webBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["DbType"] = DbTypes.InMemory.ToString(),
                        ["Seed"] = "false",
                        ["IdentityServerOptions:IssuerUri"] = "http://localhost"
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<TestUserService>();
                    services.AddControllersWithViews()
                        .AddApplicationPart(typeof(Config).Assembly);
                })
                .Configure((context, configureApp) =>
                {
                    configureApp.Use((context, next) =>
                    {
                        var testService = context.RequestServices.GetRequiredService<TestUserService>();
                        if (testService.User is not null)
                        {
                            context.User = testService.User;
                        }
                        
                        return next();
                    });

                    using var scope = configureApp.ApplicationServices.CreateScope();
                    var dbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                    if (dbContext != null && !dbContext.Providers.Any(p => p.Id == "Google"))
                    {
                        dbContext.Providers.Add(new IdentityServer.Store.Entity.ExternalProvider
                        {
                            Id = "Google",
                            DisplayName = "Google",
                            SerializedHandlerType = "{\"Name\":\"Microsoft.AspNetCore.Authentication.Google.GoogleHandler\"}",
                            SerializedOptions = "{\"ClientId\":\"818322595124-h0nd8080luc71ba2i19a5kigackfm8me.apps.googleusercontent.com\",\"ClientSecret\":\"ac_tx-O9XvZGNRi4HYfPerx2\",\"AuthorizationEndpoint\":\"https://accounts.google.com/o/oauth2/v2/auth\",\"TokenEndpoint\":\"https://oauth2.googleapis.com/token\",\"UserInformationEndpoint\":\"https://www.googleapis.com/oauth2/v2/userinfo\",\"Events\":{},\"ClaimActions\":[{\"JsonKey\":\"id\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"name\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"given_name\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"family_name\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"link\",\"ClaimType\":\"urn:google:profile\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"email\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"}],\"Scope\":[\"openid\",\"profile\",\"email\"],\"BackchannelTimeout\":\"00:01:00\",\"Backchannel\":{\"DefaultRequestHeaders\":[{\"Key\":\"User-Agent\",\"Value\":[\"Microsoft\",\"ASP.NET\",\"Core\",\"OAuth\",\"handler\"]}],\"DefaultRequestVersion\":\"1.1\",\"Timeout\":\"00:01:00\",\"MaxResponseContentBufferSize\":10485760},\"CallbackPath\":\"/signin-Google\",\"ReturnUrlParameter\":\"ReturnUrl\",\"SignInScheme\":\"Identity.External\",\"RemoteAuthenticationTimeout\":\"00:15:00\",\"CorrelationCookie\":{\"Name\":\".AspNetCore.Correlation.\",\"HttpOnly\":true,\"IsEssential\":true}}"
                        });
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch
                        {
                            // silent
                        }
                    }

                    configureApp.UseTheIdServer(context.HostingEnvironment, context.Configuration);
                });
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            catch
            {
                // silent
            }
        }
    }

    [CollectionDefinition(Name)]
    public class BlazorAppCollection : ICollectionFixture<TheIdServerFactory>
    {
        public const string Name = "blazor collection";
    }
}
