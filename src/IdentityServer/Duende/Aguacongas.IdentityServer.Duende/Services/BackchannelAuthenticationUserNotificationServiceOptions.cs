namespace Aguacongas.IdentityServer.Services
{
    public class BackchannelAuthenticationUserNotificationServiceOptions : IdentityServerOptions
    {
        public string AssemblyPath { get; set; }
        public string ServiceType { get; set; } = typeof(BackchannelAuthenticationUserNotificationService).AssemblyQualifiedName;
    }
}