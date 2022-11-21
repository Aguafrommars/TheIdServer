using System.Runtime.InteropServices;

namespace Aguacongas.TheIdServer.BlazorApp.Shared
{
    public partial class MainLayout
    {
        static bool IsClientSide => RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));
    }
}
