using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Http.Store
{
    public class AuthorizationCodeStore : IAuthorizationCodeStore
    {
        private readonly IAdminStore<Entity.AuthorizationCode> _store;
        private readonly IPersistentGrantSerializer _serializer;

        public AuthorizationCodeStore(IAdminStore<Entity.AuthorizationCode> store, 
            IPersistentGrantSerializer serializer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<AuthorizationCode> GetAuthorizationCodeAsync(string code)
        {
            code = code ?? throw new ArgumentNullException(nameof(code));

            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"Id eq '{code}'",
                Select = "Data"
            }).ConfigureAwait(false);
            if (response.Count == 1)
            {
                return _serializer.Deserialize<AuthorizationCode>(response.Items.First().Data);
            }
            return null;
        }

        public async Task RemoveAuthorizationCodeAsync(string code)
        {
            code = code ?? throw new ArgumentNullException(nameof(code));

            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"Id eq '{code}'",
                Select = "Id"
            }).ConfigureAwait(false);
            foreach(var item in response.Items)
            {
                await _store.DeleteAsync(item.Id).ConfigureAwait(false);
            }
        }

        public async Task<string> StoreAuthorizationCodeAsync(AuthorizationCode code)
        {
            code = code ?? throw new ArgumentNullException(nameof(code));

            var newEntity = new Entity.AuthorizationCode
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = code.ClientId,
                Data = _serializer.Serialize(code),
                Expiration = code.CreationTime.AddSeconds(code.Lifetime)
            };

            var entity = await _store.CreateAsync(newEntity).ConfigureAwait(false);
            return entity.Id;
        }
    }
}
