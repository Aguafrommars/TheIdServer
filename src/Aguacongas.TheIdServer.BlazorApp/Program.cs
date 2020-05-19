using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp
{
    [SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "entry point")]
    [SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "<Pending>")]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.AddTheIdServerApp();
            await builder.Build().RunAsync();
        }
    }
}
