using Aguacongas.IdentityServer.Store.Entitiy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityContext<TKey,
        TIdentity,
        TIdentityClaim,
        TIdentityProperty,
        TClaimType> : ClaimTypeContext<TKey, TClaimType>
        where TKey: IEquatable<TKey>
        where TIdentity: Identity<TKey>
        where TIdentityClaim: IdentityClaim<TKey>
        where TIdentityProperty: IdentityProperty<TKey>
        where TClaimType : ClaimType<TKey>
    {
        public DbSet<TIdentity> Identities { get; set; }

        public DbSet<TIdentityClaim> IdentityClaims { get; set; }

        public DbSet<TIdentityProperty> IdentityProperties { get; set; }
    }
}
