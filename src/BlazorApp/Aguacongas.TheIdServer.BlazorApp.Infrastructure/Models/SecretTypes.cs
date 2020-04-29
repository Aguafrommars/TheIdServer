namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public static class SecretTypes
    {
        public static string[] Values { get; } = new string[]
        {
            "SharedSecret",
            "X509Thumbprint",
            "X509Name",
            "X509CertificateBase64",
            "JWK"
        };
    }
}
