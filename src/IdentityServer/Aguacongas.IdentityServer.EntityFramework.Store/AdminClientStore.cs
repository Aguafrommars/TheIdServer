using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Log message")]
    public class AdminClientStore : IAdminStore<Client>
    {
        private readonly ClientContext _context;
        private readonly ILogger<AdminClientStore> _logger;

        public AdminClientStore(ClientContext context, ILogger<AdminClientStore> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public IQueryable<Client> Items => _context.Clients;

        public async Task<Client> CreateAsync(Client client, CancellationToken cancellationToken = default)
        {
            client = client ?? throw new ArgumentNullException(nameof(client));
            await _context.Clients.AddAsync(client, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Client {ClientId} created", client.Id, client);
            return client;
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var client = await _context.Clients.FindAsync(id).ConfigureAwait(false);
            if (client != null)
            {
                _context.Remove(client);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Client {ClientId} deleted", client.Id, client);
            }
        }

        public async Task<Client> UpdateAsync(Client client, CancellationToken cancellationToken = default)
        {
            client = client ?? throw new ArgumentNullException(nameof(client));
            _context.Clients.Update(client);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Client {ClientId} updated", client.Id, client);
            return client;
        }
    }
}
