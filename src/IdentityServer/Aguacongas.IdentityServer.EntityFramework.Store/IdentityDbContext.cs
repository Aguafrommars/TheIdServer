using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore = Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityDbContext<TUser>
        : IdentityDbContext<TUser, IdentityRole>
        where TUser : IdentityUser<string>
    {
        public IdentityDbContext(DbContextOptions options) : base(options)
        {

        }
    }

    public class IdentityDbContext<TUser, TRole>
        : EntityFrameworkCore.IdentityDbContext<TUser, TRole, string, UserClaim, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        public IdentityDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
