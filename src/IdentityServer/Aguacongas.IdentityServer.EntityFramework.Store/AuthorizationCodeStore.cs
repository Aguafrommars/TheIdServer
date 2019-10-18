using Aguacongas.IdentityServer.Store;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class AuthorizationCodeStore : IAuthorizationCodeStore
    {
        private readonly ClientContext _context;

        public AuthorizationCodeStore(ClientContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AuthorizationCode> GetAuthorizationCodeAsync(string code)
        {
            var entity = await _context.AuthorizationCodes.FindAsync(code);
            return entity.ToAuthorizationCode();
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

            var newEntity = code.ToEntity(client);            
            await _context.AuthorizationCodes.AddAsync(newEntity);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return newEntity.Id;
        }
    }
}
