using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class UserConsentStore : IUserConsentStore
    {
        private readonly ClientContext _context;

        public UserConsentStore(ClientContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
        {
            var entity = await _context.UserConstents
                .FirstOrDefaultAsync(c => c.SubjectId == subjectId & c.Client.Id == clientId)
                .ConfigureAwait(false);

            return entity.ToConsent();
        }

        public async Task RemoveUserConsentAsync(string subjectId, string clientId)
        {
            var entity = await _context.UserConstents
                .FirstOrDefaultAsync(c => c.SubjectId == subjectId & c.Client.Id == clientId)
                .ConfigureAwait(false);

            if (entity != null)
            {
                _context.UserConstents.Remove(entity);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task StoreUserConsentAsync(Consent consent)
        {
            consent = consent ?? throw new ArgumentNullException(nameof(consent));
            var client = await _context.Clients.FindAsync(consent.ClientId);
            if (client == null)
            {
                throw new InvalidOperationException($"Client {consent.ClientId} not found");
            }

            var newEntity = consent.ToEntity(client);

            var entity = await _context.UserConstents
                .FirstOrDefaultAsync(c => c.SubjectId == consent.SubjectId && c.Client.Id == consent.ClientId)
                .ConfigureAwait(false);

            if (entity == null)
            {
                await _context.UserConstents.AddAsync(newEntity);
            }
            else
            {
                newEntity.Id = entity.Id;
                _context.Update(newEntity);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
