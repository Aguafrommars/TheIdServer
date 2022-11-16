#if DUENDE
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
#else
using IdentityServer4.Models;
using IdentityServer4.Stores;
#endif
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.KeysRotation
{
    internal class ValidattionKeysStore : IValidationKeysStore
    {
        public ICacheableKeyRingProvider _keyringProvider;

        public ValidattionKeysStore(ICacheableKeyRingProvider keyringProvider)
        {
            _keyringProvider = keyringProvider ?? throw new ArgumentNullException(nameof(keyringProvider));
        }

        public Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            var keyInfos = _keyringProvider.GetAllKeys().Where(k => !k.IsRevoked);

            return Task.FromResult(keyInfos.Select(i =>
            {
                if (i.Descriptor is RsaEncryptorDescriptor rsa)
                {
                    return CreateRsaSinginKey(rsa);
                }

                return CreateEcdSingingKey(i);
            }));
        }

        private SecurityKeyInfo CreateEcdSingingKey(IKey i)
        {
            var ecd = i.Descriptor as ECDsaEncryptorDescriptor;
            var algorythm = ecd.Configuration.SigningAlgorithm?.ToString() ?? _keyringProvider.Algorithm;
            var key = ecd.ECDsaSecurityKey;
            return new SecurityKeyInfo
            {
                Key = key,
                SigningAlgorithm = algorythm
            };
        }

        private SecurityKeyInfo CreateRsaSinginKey(RsaEncryptorDescriptor rsa)
        {
            var algorythm = rsa.Configuration.SigningAlgorithm?.ToString() ?? _keyringProvider.Algorithm;
            var key = rsa.RsaSecurityKey;
            return new SecurityKeyInfo
            {
                SigningAlgorithm = algorythm,
                Key = key
            };
        }
    }
}
