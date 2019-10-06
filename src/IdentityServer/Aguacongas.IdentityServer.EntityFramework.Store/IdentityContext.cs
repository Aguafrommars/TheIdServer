using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityContext : IdentityContext<Identity>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }
    }

    public class IdentityContext<TIdentity> : IdentityContext<string, TIdentity>
        where TIdentity : Identity<string>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }
    }

    public class IdentityContext<TKey, TIdentity> : IdentityContext<
        TKey,
        TIdentity,
        IdentityClaim<TKey>,
        IdentityProperty<TKey>>
        where TKey : IEquatable<TKey>
        where TIdentity : Identity<TKey>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }
    }
    public class IdentityContext<TKey,
        TIdentity,
        TIdentityClaim,
        TIdentityProperty> : DbContext
        where TKey: IEquatable<TKey>
        where TIdentity: Identity<TKey>
        where TIdentityClaim: IdentityClaim<TKey>
        where TIdentityProperty: IdentityProperty<TKey>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TIdentity> Identities { get; set; }

        public DbSet<TIdentityClaim> IdentityClaims { get; set; }

        public DbSet<TIdentityProperty> IdentityProperties { get; set; }
    }
}
