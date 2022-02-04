// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
#if DUENDE
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using models = Duende.IdentityServer.Models;
#else
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using models = IdentityServer4.Models;
#endif
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
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

        public async Task<models.DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            deviceCode = deviceCode ?? throw new ArgumentNullException(nameof(deviceCode));

            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(DeviceCode.Code)} eq '{deviceCode}'",
                Select = nameof(DeviceCode.Data)
            }).ConfigureAwait(false);

            if (response.Items.Any())
            {
                return ToModel(response.Items.First());
            }

            return null;
        }

        public async Task<models.DeviceCode> FindByUserCodeAsync(string userCode)
        {
            userCode = userCode ?? throw new ArgumentNullException(nameof(userCode));

            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(DeviceCode.UserCode)} eq '{userCode}'",
                Select = nameof(DeviceCode.Data)
            }).ConfigureAwait(false);

            if (response.Items.Any())
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
                Filter = $"{nameof(DeviceCode.Code)} eq '{deviceCode}'",
                Select = nameof(DeviceCode.Id)
            }).ConfigureAwait(false);


            foreach(var entity in response.Items)
            {
                await _store.DeleteAsync(entity.Id).ConfigureAwait(false);
            }
        }

        public Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, models.DeviceCode data)
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

        public async Task UpdateByUserCodeAsync(string userCode, models.DeviceCode data)
        {
            userCode = userCode ?? throw new ArgumentNullException(nameof(userCode));
            data = data ?? throw new ArgumentNullException(nameof(data));

            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(DeviceCode.UserCode)} eq '{userCode}'"
            }).ConfigureAwait(false);

            if (response.Items.Any())
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

        private models.DeviceCode ToModel(DeviceCode entity)
        {
            if (entity != null)
            {
                return _serializer.Deserialize<models.DeviceCode>(entity.Data);
            }

            return null;
        }
    }
}
