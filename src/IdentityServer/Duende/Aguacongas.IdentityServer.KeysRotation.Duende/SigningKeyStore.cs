using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services.KeyManagement;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.KeysRotation.Duende
{
    internal class SigningKeyStore<TC, TE> : ISigningKeyStore
        where TC : SigningAlgorithmConfiguration
        where TE : ISigningAlgortithmEncryptor
    {
        static JsonSerializerOptions _settings = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        public ICacheableKeyRingProvider<TC, TE> _keyringProvider;

        public SigningKeyStore(ICacheableKeyRingProvider<TC, TE> keyringProvider)
        {
            _keyringProvider = keyringProvider ?? throw new ArgumentNullException(nameof(keyringProvider));
        }

        public Task DeleteKeyAsync(string id)
        => Task.CompletedTask;
        

        public Task<IEnumerable<SerializedKey>> LoadKeysAsync()
        {
            var keyInfos = _keyringProvider.KeyManager.GetAllKeys().Where(k => !k.IsRevoked);
            if (typeof(TC) == typeof(RsaEncryptorConfiguration))
            {
                keyInfos = keyInfos.Where(k => k.Descriptor is RsaEncryptorDescriptor);
            }
            else
            {
                keyInfos = keyInfos.Where(k => k.Descriptor is ECDsaEncryptorDescriptor);
            }

            return Task.FromResult(keyInfos.Select(i =>
            {
                if (i.Descriptor is RsaEncryptorDescriptor rsa)
                {
                    return CreateRsaSinginKey(i, rsa);
                }

                return CreateEcdSingingKey(i);
            }));
        }

        public Task StoreKeyAsync(SerializedKey key)
        => throw new NotImplementedException();

        private static SerializedKey CreateEcdSingingKey(IKey i)
        {
            var ecd = i.Descriptor as ECDsaEncryptorDescriptor;
            var algorythm = ecd.Configuration.SigningAlgorithm.ToString();
            var key = ecd.ECDsaSecurityKey;
            var created = i.CreationDate.UtcDateTime;
            return new SerializedKey
            {
                Algorithm = algorythm,
                Created = created,
                Id = key.KeyId,
                DataProtected = false,
                Data = JsonSerializer.Serialize(new EcKeyContainer(key, algorythm, created))
            };
        }

        private static SerializedKey CreateRsaSinginKey(IKey i, RsaEncryptorDescriptor rsa)
        {
            var algorythm = rsa.Configuration.SigningAlgorithm.ToString();
            var key = rsa.RsaSecurityKey;
            var created = i.CreationDate.UtcDateTime;
            return new SerializedKey
            {
                Algorithm = algorythm,
                Created = created,
                Id = key.KeyId,
                DataProtected = false,
                Data = JsonSerializer.Serialize(new RsaKeyContainer
                {
                    Algorithm = algorythm,
                    Created = created,
                    Id = key.KeyId,
                    Parameters = key.Parameters
                }, _settings)
            };
        }
    }
}
