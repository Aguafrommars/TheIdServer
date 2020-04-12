using Aguacongas.IdentityServer.Http.Store;
using Aguacongas.TheIdServer.Areas.Identity.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http;

[assembly: HostingStartup(typeof(Aguacongas.TheIdServer.Areas.Identity.IdentityHostingStartup))]
namespace Aguacongas.TheIdServer.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.Configure<IdentityServerOptions>("EmailOptions", context.Configuration.GetSection("EmailApiAuthentication"))
                    .AddSingleton(p =>
                    {
                        using var scope = p.CreateScope();

                        return new EmailOAuthTokenManager(new HttpClient(),
                            new EmailOptions(scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<IdentityServerOptions>>()));
                    })
                    .AddTransient<IEmailSender>(p =>
                    {
                        var factory = p.GetRequiredService<IHttpClientFactory>();
                        var options = p.GetRequiredService<IOptionsSnapshot<IdentityServerOptions>>().Get("EmailOptions");
                        return new EmailApiSender(factory.CreateClient(options.HttpClientName), options);
                    })
                    .AddTransient<EmailOAuthDelegatingHandler>()
                    .AddHttpClient(context.Configuration.GetValue<string>("EmailApiAuthentication:HttpClientName"))
                    .AddHttpMessageHandler<EmailOAuthDelegatingHandler>();
            });
        }

        class EmailOptions : IOptions<IdentityServerOptions>
        {
            private readonly IOptionsSnapshot<IdentityServerOptions> _optionsSnapshot;

            public EmailOptions(IOptionsSnapshot<IdentityServerOptions> optionsSnapshot)
            {
                _optionsSnapshot = optionsSnapshot;
            }

            public IdentityServerOptions Value => _optionsSnapshot.Get("EmailOptions");
        }

        class EmailOAuthDelegatingHandler : OAuthDelegatingHandler
        {
            public EmailOAuthDelegatingHandler(EmailOAuthTokenManager manager)
                : base(manager)
            {
            }
        }

        class EmailOAuthTokenManager : OAuthTokenManager
        {
            public EmailOAuthTokenManager(HttpClient httpClient, IOptions<IdentityServerOptions> options)
                : base(httpClient, options)
            {
            }
        }
    }
}