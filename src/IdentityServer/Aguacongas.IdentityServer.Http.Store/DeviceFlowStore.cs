using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Models = IdentityServer4.Models;

namespace Aguacongas.IdentityServer.Http.Store
{
    public class DeviceFlowStore : IDeviceFlowStore
    {
        private readonly IAdminStore<DeviceCode> _store;
        private readonly IPersistentGrantSerializer _serializer;

        public DeviceFlowStore(IAdminStore<DeviceCode> store,
            IPersistentGrantSerializer serializer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<Models.DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            deviceCode = deviceCode ?? throw new ArgumentNullException(nameof(deviceCode));

            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"Code eq '{deviceCode}'",
                Select = "Data"
            }).ConfigureAwait(false);

            if (response.Count == 1)
            {
                return ToModel(response.Items.First());
            }

            return null;
        }

        public async Task<Models.DeviceCode> FindByUserCodeAsync(string userCode)
        {
            userCode = userCode ?? throw new ArgumentNullException(nameof(userCode));

            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"UserCode eq '{userCode}'",
                Select = "Data"
            }).ConfigureAwait(false);

            if (response.Count == 1)
            {
                return ToModel(response.Items.First());
            }

            return null;
        }

        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            deviceCode = deviceCode ?? throw new ArgumentNullException(nameof(deviceCode));

            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"Code eq '{deviceCode}'",
                Select = "Id"
            }).ConfigureAwait(false);


            foreach(var entity in response.Items)
            {
                await _store.DeleteAsync(entity.Id).ConfigureAwait(false);
            }
        }

        public Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, Models.DeviceCode data)
        {
            deviceCode = deviceCode ?? throw new ArgumentNullException(nameof(deviceCode));
            userCode = userCode ?? throw new ArgumentNullException(nameof(userCode));
            data = data ?? throw new ArgumentNullException(nameof(data));

            var entity = new DeviceCode
            {
                Code = deviceCode,
                UserCode = userCode,
                Data = _serializer.Serialize(data),
                ClientId = data.ClientId,
                SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject).Value,
                Expiration = data.CreationTime.AddSeconds(data.Lifetime),
            };

            return _store.CreateAsync(entity);
        }

        public async Task UpdateByUserCodeAsync(string userCode, Models.DeviceCode data)
        {
            userCode = userCode ?? throw new ArgumentNullException(nameof(userCode));
            data = data ?? throw new ArgumentNullException(nameof(data));

            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"UserCode eq '{userCode}'"
            }).ConfigureAwait(false);

            if (response.Count == 1)
            {
                var entity = response.Items.First();
                entity.Data = _serializer.Serialize(data);
                entity.Expiration = data.CreationTime.AddSeconds(data.Lifetime);
                entity.SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject).Value;
                await _store.UpdateAsync(entity).ConfigureAwait(false);
                return;
            }

            throw new InvalidOperationException($"Device code for {userCode} not found");         
        }

        private Models.DeviceCode ToModel(DeviceCode entity)
        {
            if (entity != null)
            {
                return _serializer.Deserialize<Models.DeviceCode>(entity.Data);
            }

            return null;
        }
    }
}
