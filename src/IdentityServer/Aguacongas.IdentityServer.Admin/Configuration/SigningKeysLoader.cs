// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Aguacongas.IdentityServer.Admin.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static class SigningKeysLoader
    {
        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="password">The password.</param>
        /// <param name="keyStorageFlags">The key storage flags.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// There was an error loading the certificate. The file '{path}' was not found.
        /// or
        /// There was an error loading the certificate. No password was provided.
        /// or
        /// </exception>
        public static X509Certificate2 LoadFromFile(string path, string password, X509KeyStorageFlags keyStorageFlags)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new InvalidOperationException($"There was an error loading the certificate. The file '{path}' was not found.");
                }
                else if (password == null)
                {
                    throw new InvalidOperationException("There was an error loading the certificate. No password was provided.");
                }

                return new X509Certificate2(path, password, keyStorageFlags);
            }
            catch (CryptographicException e)
            {
                var message = "There was an error loading the certificate. Either the password is incorrect or the process does not have permisions to " +
                    $"store the key in the Keyset '{keyStorageFlags}'";
                throw new InvalidOperationException(message, e);
            }
        }

        /// <summary>
        /// Loads from store cert.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="currentTime">The current time.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Couldn't find a valid certificate with " +
        ///                         $"subject '{subject}' on the '{storeLocation}\\{storeName}'</exception>
        public static X509Certificate2 LoadFromStoreCert(
            string subject,
            string storeName,
            StoreLocation storeLocation,
            DateTimeOffset currentTime)
        {
            using var store = new X509Store(storeName, storeLocation);
            X509Certificate2Collection storeCertificates = null;
            X509Certificate2 foundCertificate = null;

            try
            {
                store.Open(OpenFlags.ReadOnly);
                storeCertificates = store.Certificates;
                var foundCertificates = storeCertificates
                    .Find(X509FindType.FindBySubjectDistinguishedName, subject, validOnly: false);

                foundCertificate = foundCertificates
                    .OfType<X509Certificate2>()
                    .Where(certificate => certificate.NotBefore <= currentTime && certificate.NotAfter > currentTime)
                    .OrderBy(certificate => certificate.NotAfter)
                    .FirstOrDefault();

                if (foundCertificate == null)
                {
                    throw new InvalidOperationException("Couldn't find a valid certificate with " +
                        $"subject '{subject}' on the '{storeLocation}\\{storeName}'");
                }

                return foundCertificate;
            }
            finally
            {
                DisposeCertificates(storeCertificates, except: foundCertificate);
            }
        }

        /// <summary>
        /// Loads the development.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="createIfMissing">if set to <c>true</c> [create if missing].</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Couldn't find the file '{path}' and creation of a development key was not requested.</exception>
        public static RSA LoadDevelopment(string path, bool createIfMissing)
        {
            var fileExists = File.Exists(path);
            if (!fileExists && !createIfMissing)
            {
                throw new InvalidOperationException($"Couldn't find the file '{path}' and creation of a development key was not requested.");
            }

            if (fileExists)
            {
                var rsa = JsonConvert.DeserializeObject<RsaKeyParameters>(File.ReadAllText(path));
                return rsa.GetRSA();
            }
            else
            {
                var parameters = RsaKeyParameters.Create();
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(path, JsonConvert.SerializeObject(parameters));
                return parameters.GetRSA();
            }
        }

        private class RsaKeyParameters
        {
            public string D { get; set; }
            public string DP { get; set; }
            public string DQ { get; set; }
            public string E { get; set; }
            public string IQ { get; set; }
            public string M { get; set; }
            public string P { get; set; }
            public string Q { get; set; }

            public static RsaKeyParameters Create()
            {
                using var rsa = RSA.Create();
                if (rsa is RSACryptoServiceProvider rSACryptoServiceProvider && rsa.KeySize < 2048)
                {
                    rsa.KeySize = 2048;
                    if (rsa.KeySize < 2048)
                    {
                        throw new InvalidOperationException("We can't generate an RSA key with at least 2048 bits. Key generation is not supported in this system.");
                    }
                }

                return GetParameters(rsa);
            }

            public static RsaKeyParameters GetParameters(RSA key)
            {
                var result = new RsaKeyParameters();
                var rawParameters = key.ExportParameters(includePrivateParameters: true);

                if (rawParameters.D != null)
                {
                    result.D = Convert.ToBase64String(rawParameters.D);
                }

                if (rawParameters.DP != null)
                {
                    result.DP = Convert.ToBase64String(rawParameters.DP);
                }

                if (rawParameters.DQ != null)
                {
                    result.DQ = Convert.ToBase64String(rawParameters.DQ);
                }

                if (rawParameters.Exponent != null)
                {
                    result.E = Convert.ToBase64String(rawParameters.Exponent);
                }

                if (rawParameters.InverseQ != null)
                {
                    result.IQ = Convert.ToBase64String(rawParameters.InverseQ);
                }

                if (rawParameters.Modulus != null)
                {
                    result.M = Convert.ToBase64String(rawParameters.Modulus);
                }

                if (rawParameters.P != null)
                {
                    result.P = Convert.ToBase64String(rawParameters.P);
                }

                if (rawParameters.Q != null)
                {
                    result.Q = Convert.ToBase64String(rawParameters.Q);
                }

                return result;
            }

            public RSA GetRSA()
            {
                var parameters = new RSAParameters();
                if (D != null)
                {
                    parameters.D = Convert.FromBase64String(D);
                }

                if (DP != null)
                {
                    parameters.DP = Convert.FromBase64String(DP);
                }

                if (DQ != null)
                {
                    parameters.DQ = Convert.FromBase64String(DQ);
                }

                if (E != null)
                {
                    parameters.Exponent = Convert.FromBase64String(E);
                }

                if (IQ != null)
                {
                    parameters.InverseQ = Convert.FromBase64String(IQ);
                }

                if (M != null)
                {
                    parameters.Modulus = Convert.FromBase64String(M);
                }

                if (P != null)
                {
                    parameters.P = Convert.FromBase64String(P);
                }

                if (Q != null)
                {
                    parameters.Q = Convert.FromBase64String(Q);
                }

                var rsa = RSA.Create();
                rsa.ImportParameters(parameters);

                return rsa;
            }
        }

        private static void DisposeCertificates(X509Certificate2Collection certificates, X509Certificate2 except)
        {
            if (certificates != null)
            {
                foreach (var certificate in certificates)
                {
                    if (!certificate.Equals(except))
                    {
                        certificate.Dispose();
                    }
                }
            }
        }
    }
}
