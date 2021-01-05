// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public static class SecretTypes
    {
        public static readonly string SharedSecret = "SharedSecret";
        public static readonly string X509Thumbprint = "X509Thumbprint";
        public static readonly string X509Name = "X509Name";
        public static readonly string X509CertificateBase64 = "X509CertificateBase64";
        public static readonly string JWK = "JWK";

        public static string[] Values { get; } = new string[]
        {
            SharedSecret,
            X509Thumbprint,
            X509Name,
            X509CertificateBase64,
            JWK
        };
    }
}
