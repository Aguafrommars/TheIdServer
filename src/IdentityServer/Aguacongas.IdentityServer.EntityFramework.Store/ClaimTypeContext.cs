using Aguacongas.IdentityServer.Store.Entitiy;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public abstract class ClaimTypeContext<TKey, TClaimType>:DbContext 
        where TKey: IEquatable<TKey>
        where TClaimType : ClaimType<TKey>
    {
        public ClaimTypeContext(DbContextOptions options):base(options)
        { }

        protected ClaimTypeContext() { }

        public virtual DbSet<ClaimType<TKey>> ClaimTypes { get; set; }

    }
}
