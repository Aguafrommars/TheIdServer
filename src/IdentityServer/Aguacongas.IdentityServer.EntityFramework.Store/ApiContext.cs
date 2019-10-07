using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{

    public class ApiDbContext : DbContext
    { 
        public ApiDbContext(DbContextOptions options): base(options)
        { }

        protected ApiDbContext() { }

        public virtual DbSet<Api> Apis { get; set; }

        public virtual DbSet<ApiClaim> ApiClaims { get; set; }

        public virtual DbSet<ApiScope> ApiScopes { get; set; }

        public virtual DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        public virtual DbSet<ApiSecret> ApiSecrets { get; set; }
    }
}
