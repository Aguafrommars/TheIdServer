using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class KeyManagementOptions
    {
        //
        // Summary:
        //     Specifies whether the data protection system should auto-generate keys.
        //
        // Remarks:
        //     If this value is 'false', the system will not generate new keys automatically.
        //     The key ring must contain at least one active non-revoked key, otherwise calls
        //     to Microsoft.AspNetCore.DataProtection.IDataProtector.Protect(System.Byte[])
        //     may fail. The system may end up protecting payloads to expired keys if this property
        //     is set to 'false'. The default value is 'true'.
        public bool AutoGenerateKeys { get; set; }

        //
        // Summary:
        //     Controls the lifetime (number of days before expiration) for newly-generated
        //     keys.
        //
        // Remarks:
        //     The lifetime cannot be less than one week. The default value is 90 days.
        public TimeSpan NewKeyLifetime { get; set; }

    }
}