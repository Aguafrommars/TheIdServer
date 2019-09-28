using Aguacongas.IdentityServer.Store.Entitiy;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ClaimTypeContext<TKey, TClaimType>:DbContext 
        where TKey: IEquatable<TKey>
        where TClaimType : ClaimType<TKey>
    {
        public DbSet<ClaimType<TKey>> ClaimTypes { get; set; }

    }
}
