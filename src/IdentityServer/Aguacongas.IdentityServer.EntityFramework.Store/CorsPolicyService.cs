using Aguacongas.IdentityServer.Store;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    /// <summary>
    /// <see cref="ICorsPolicyService"/> implementation
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.ICorsPolicyService" />
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IdentityServerDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsPolicyService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public CorsPolicyService(IdentityServerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var corsUri = new Uri(origin);
            var corsValue = (int)IdentityServer.Store.Entity.UriKind.Cors;
            var corsUris = await _context.ClientUris
                .Where(o => (o.Kind & corsValue) == corsValue)
                .ToListAsync()
                .ConfigureAwait(false);
            return corsUris.Any(o => corsUri.CorsMatch(o.Uri));
        }

    }
}
