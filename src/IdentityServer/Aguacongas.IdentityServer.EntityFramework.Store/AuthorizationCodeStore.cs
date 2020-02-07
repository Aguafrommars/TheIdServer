using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class AuthorizationCodeStore : AdminStore<Entity.AuthorizationCode,  OperationalDbContext>, IAuthorizationCodeStore
    {
        private readonly OperationalDbContext _context;
        private readonly IPersistentGrantSerializer _serializer;

        public AuthorizationCodeStore(OperationalDbContext context, 
            IPersistentGrantSerializer serializer,
            ILogger<AuthorizationCodeStore> logger)
            : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AuthorizationCode> GetAuthorizationCodeAsync(string code)
        {
            var entity = await _context.AuthorizationCodes.FindAsync(code);
            if (entity != null)
            {
                return _serializer.Deserialize<AuthorizationCode>(entity.Data);
            }
            return null;
        }

        public async Task RemoveAuthorizationCodeAsync(string code)
        {
            var entity = await _context.AuthorizationCodes.FindAsync(code);
            if (entity != null)
            {
                try
                {
                    _context.AuthorizationCodes.Remove(entity);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                { }
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
            await _context.AuthorizationCodes.AddAsync(newEntity);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return newEntity.Id;
        }
    }
}
