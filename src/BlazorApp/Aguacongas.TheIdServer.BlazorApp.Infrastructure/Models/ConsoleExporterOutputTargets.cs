using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    [Flags]
    public enum ConsoleExporterOutputTargets
    {
        //
        // Summary:
        //     Output to the Console (stdout).
        Console = 0x1,
        //
        // Summary:
        //     Output to the Debug trace.
        Debug = 0x2
    }
}