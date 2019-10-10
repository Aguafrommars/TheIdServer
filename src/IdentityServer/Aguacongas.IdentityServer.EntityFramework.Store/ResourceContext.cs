using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{

    public class ResourceContext : AuditableDbContext
    { 
        public ResourceContext(DbContextOptions<ResourceContext> options): base(options)
        { }

        public virtual DbSet<Api> Apis { get; set; }

        public virtual DbSet<ApiClaim> ApiClaims { get; set; }

        public virtual DbSet<ApiScope> ApiScopes { get; set; }

        public virtual DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        public virtual DbSet<ApiSecret> ApiSecrets { get; set; }

        public virtual DbSet<Identity> Identities { get; set; }

        public virtual DbSet<IdentityClaim> IdentityClaims { get; set; }

        public virtual DbSet<IdentityProperty> IdentityProperties { get; set; }
    }
}
