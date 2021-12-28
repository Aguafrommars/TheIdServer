namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public enum KeyProtectionKind
    {
        None,
        AzureKeyVault,
        WindowsDpApi,
        X509,
        WindowsDpApiNg
    }

    public class KeyProtectionOptions
    {
        public KeyProtectionKind KeyProtectionKind { get; set; }

        public string X509CertificateThumbprint { get; set; }

        public string AzureKeyVaultKeyId { get; set; }

        public string AzureKeyVaultClientId { get; set; }

        public string AzureKeyVaultClientSecret { get; set; }

        public bool WindowsDPAPILocalMachine { get; set; }

        public string WindowsDpApiNgSid { get; set; }

        public string WindowsDpApiNgCerticate { get; set; }
        public string X509CertificatePath { get; set; }
        public string X509CertificatePassword { get; set; }
    }
}