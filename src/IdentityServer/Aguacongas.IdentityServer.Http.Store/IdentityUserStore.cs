using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    public class IdentityUserStore : AdminStore<IdentityUser>, IAdminStore<IdentityUser>
    {
        public IdentityUserStore(Task<HttpClient> httpClientFactory, ILogger<AdminStore<IdentityUser>> logger) : base(httpClientFactory, logger)
        {
        }

        public Task<IEntityId> CreateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEntityId> UpdateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
