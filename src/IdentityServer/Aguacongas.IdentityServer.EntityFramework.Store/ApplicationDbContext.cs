using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Models;
using Microsoft.EntityFrameworkCore;

namespace Aguacongas.TheIdServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
