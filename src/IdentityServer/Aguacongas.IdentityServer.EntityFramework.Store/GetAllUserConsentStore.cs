using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class GetAllUserConsentStore : IGetAllUserConsentStore
    {
        private readonly IdentityServerDbContext _context;
        private readonly IPersistentGrantSerializer _serializer;

        public GetAllUserConsentStore(IdentityServerDbContext context, IPersistentGrantSerializer serializer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Consent>> GetAllUserConsent(string subjectId)
        {
            return await _context.UserConstents
                .Where(c => c.UserId == subjectId)
                .Select(c => _serializer.Deserialize<Consent>(c.Data))
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
