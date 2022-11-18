// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
using Duende.IdentityServer.Stores;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// 
    /// </summary>
    public static class SecurityKeyExtensions
    {
        /// <summary>
        /// Gets the X509 certificate.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="store">The store.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Cannot use signing credential with key of type '{key.GetType().Name}'</exception>
        /// <exception cref="InvalidOperationException">Cannot use signing credential with key of type '{key.GetType().Name}'</exception>
        public static X509Certificate2 GetX509Certificate(this SecurityKey key, ISigningCredentialStore store)
        {
            if (key is RsaSecurityKey rsaKey)
            {
                var rsa = rsaKey.Rsa ?? RSA.Create(rsaKey.Parameters);
                var certRequest = new CertificateRequest("cn=theidserver", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                if (store is IKeyRingStore keyRingStore)
                {
                    var defaultKey = keyRingStore.DefaultKey;
                    return certRequest.CreateSelfSigned(defaultKey.ActivationDate, defaultKey.ExpirationDate);                    
                }
                return certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));
            }
            else if (key is X509SecurityKey x509)
            {
                return x509.Certificate;
            }
            else
            {
                throw new InvalidOperationException($"Cannot use signing credential with key of type '{key.GetType().Name}'");
            }
        }
    }
}
