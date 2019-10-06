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
        private readonly ClientContext _context;

        public ClientStore(ClientContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var entity = await _context.Clients
                .Include(c => c.AllowedCorsOrigins)
                .Include(c => c.AllowedGrantTypes)
                .Include(c => c.AllowedScopes)
                .Include(c => c.Claims)
                .Include(c => c.ClientSecrets)
                .Include(c => c.IdentityProviderRestrictions)
                .Include(c => c.PostLogoutRedirectUris)
                .Include(c => c.Properties)
                .Include(c => c.RedirectUris)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            return entity.ToClient();
        }
    }
}
