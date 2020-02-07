using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Models = IdentityServer4.Models;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class DeviceFlowStore : AdminStore<DeviceCode, OperationalDbContext>, IDeviceFlowStore
    {
        private readonly OperationalDbContext _context;
        private readonly IPersistentGrantSerializer _serializer;

        public DeviceFlowStore(OperationalDbContext context,
            IPersistentGrantSerializer serializer,
            ILogger<DeviceFlowStore> logger)
            : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Models.DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            deviceCode = deviceCode ?? throw new ArgumentNullException(nameof(deviceCode));

            var entity = await _context.DeviceCodes.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Code == deviceCode)
                .ConfigureAwait(false);

            return ToModel(entity);
        }

        public async Task<Models.DeviceCode> FindByUserCodeAsync(string userCode)
        {
            userCode = userCode ?? throw new ArgumentNullException(nameof(userCode));

            var entity = await _context.DeviceCodes.AsNoTracking()
                .FirstOrDefaultAsync(d => d.UserCode == userCode)
                .ConfigureAwait(false);
            return ToModel(entity);
        }

        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            deviceCode = deviceCode ?? throw new ArgumentNullException(nameof(deviceCode));

            var entity = await _context.DeviceCodes
                .FirstOrDefaultAsync(d => d.Code == deviceCode)
                .ConfigureAwait(false);

            if (entity != null)
            {
                _context.DeviceCodes.Remove(entity);
                await _context.SaveChangesAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, Models.DeviceCode data)
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

            await _context.DeviceCodes.AddAsync(entity).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateByUserCodeAsync(string userCode, Models.DeviceCode data)
        {
            userCode = userCode ?? throw new ArgumentNullException(nameof(userCode));
            data = data ?? throw new ArgumentNullException(nameof(data));

            var entity = await _context.DeviceCodes
                .FirstOrDefaultAsync(d => d.UserCode == userCode)
                .ConfigureAwait(false);

            if (entity == null)
            {
                throw new InvalidOperationException($"Device code for {userCode} not found");
            }

            entity.Data = _serializer.Serialize(data);
            entity.Expiration = data.CreationTime.AddSeconds(data.Lifetime);
            entity.SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject).Value;

            await _context.SaveChangesAsync().ConfigureAwait(false);
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
