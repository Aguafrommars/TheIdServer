using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ClientStore : IClientStore
    {
        private readonly IdentityServerDbContext _context;

        public ClientStore(IdentityServerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var entity = await _context.Clients
                .Include(c => c.AllowedGrantTypes)
                .Include(c => c.AllowedScopes)
                .Include(c => c.ClientClaims)
                .Include(c => c.ClientSecrets)
                .Include(c => c.IdentityProviderRestrictions)
                .Include(c => c.Properties)
                .Include(c => c.RedirectUris)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == clientId).ConfigureAwait(false);
            return entity.ToClient();
        }
    }
}
