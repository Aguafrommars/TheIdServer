using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services.KeyManagement;
using Duende.IdentityServer.Stores;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.KeysRotation.Duende
{
    internal class SigningKeyStore : ISigningKeyStore
    {
        static JsonSerializerOptions _settings = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        public ICacheableKeyRingProvider _keyringProvider;

        public SigningKeyStore(ICacheableKeyRingProvider keyringProvider)
        {
            _keyringProvider = keyringProvider ?? throw new ArgumentNullException(nameof(keyringProvider));
        }

        public Task DeleteKeyAsync(string id)
        => throw new NotImplementedException();
        

        public Task<IEnumerable<SerializedKey>> LoadKeysAsync()
        {
            var keyInfos = _keyringProvider.KeyManager.GetAllKeys().Where(k => !k.IsRevoked);

            return Task.FromResult(keyInfos.Select(i => {
                var descriptpr = i.Descriptor as RsaEncryptorDescriptor;
                var algorythm = descriptpr.Configuration.RsaSigningAlgorithm.ToString();
                var key = descriptpr.RsaSecurityKey;
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
            }));
        }

        public Task StoreKeyAsync(SerializedKey key)
        => throw new NotImplementedException();
    }
}
