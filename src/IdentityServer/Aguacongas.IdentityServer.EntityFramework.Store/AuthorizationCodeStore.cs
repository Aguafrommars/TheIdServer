using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using System;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class AuthorizationCodeStore : IAuthorizationCodeStore
    {
        private readonly IdentityServerDbContext _context;
        private readonly IPersistentGrantSerializer _serializer;

        public AuthorizationCodeStore(IdentityServerDbContext context, IPersistentGrantSerializer serializer)
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
                _context.AuthorizationCodes.Remove(entity);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<string> StoreAuthorizationCodeAsync(AuthorizationCode code)
        {
            code = code ?? throw new ArgumentNullException(nameof(code));

            var client = await _context.Clients.FindAsync(code.ClientId);
            if (client == null)
            {
                throw new InvalidOperationException($"Client {code.ClientId} not found");
            }

            var newEntity = new Entity.AuthorizationCode
            {
                Id = Guid.NewGuid().ToString(),
                Client = client,
                Data = _serializer.Serialize(code)
            };
            await _context.AuthorizationCodes.AddAsync(newEntity);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return newEntity.Id;
        }
    }
}
