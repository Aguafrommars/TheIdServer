using System.Runtime.InteropServices;

namespace Aguacongas.TheIdServer.BlazorApp.Shared
{
    public partial class MainLayout
    {
        bool IsClientSide => RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));
    }
}
