using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Aguacongas.TheIdServer.Areas.Identity.IdentityHostingStartup))]
namespace Aguacongas.TheIdServer.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.Configure<AuthMessageSenderOptions>(context.Configuration);
                services.AddTransient<IEmailSender, EmailSender>();
            });
        }
    }
}