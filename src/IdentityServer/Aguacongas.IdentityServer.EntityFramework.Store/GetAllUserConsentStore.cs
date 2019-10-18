using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class GetAllUserConsentStore : IGetAllUserConsentStore
    {
        private readonly ClientContext _context;

        public GetAllUserConsentStore(ClientContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Consent>> GetAllUserConsent(string subjectId)
        {
            return await _context.UserConstents
                .Where(c => c.SubjectId == subjectId)
                .Select(c => c.ToConsent())
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
