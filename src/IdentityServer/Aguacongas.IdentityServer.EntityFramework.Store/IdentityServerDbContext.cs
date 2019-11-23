using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityServerDbContext : DbContext
    {
        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options):base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<ClientClaim> ClientClaims { get; set; }

        public virtual DbSet<ClientGrantType> ClientGrantTypes { get; set; }

        public virtual DbSet<ClientProperty> ClientProperties { get; set; }

        public virtual DbSet<ClientUri> ClientUris { get; set; }

        public virtual DbSet<ClientScope> ClientScopes { get; set; }

        public virtual DbSet<ClientSecret> ClientSecrets { get; set; }

        public virtual DbSet<AuthorizationCode> AuthorizationCodes { get; set; }

        public virtual DbSet<ReferenceToken> ReferenceTokens { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<UserConsent> UserConstents { get; set; }
        
        public virtual DbSet<DeviceCode> DeviceCodes { get; set; }

        public virtual DbSet<ProtectResource> Apis { get; set; }

        public virtual DbSet<ApiClaim> ApiClaims { get; set; }

        public virtual DbSet<ApiScope> ApiScopes { get; set; }

        public virtual DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        public virtual DbSet<ApiSecret> ApiSecrets { get; set; }

        public virtual DbSet<IdentityResource> Identities { get; set; }

        public virtual DbSet<IdentityClaim> IdentityClaims { get; set; }

        public virtual DbSet<IdentityProperty> IdentityProperties { get; set; }

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Cannot be null")]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientUri>()
                .HasIndex(e => new { e.ClientId, e.Uri })
                .IsUnique(true);
            modelBuilder.Entity<ClientScope>()
                .HasIndex(e => new { e.ClientId, e.Scope })
                .IsUnique(true);
            modelBuilder.Entity<ClientProperty>()
                .HasIndex(e => new { e.ClientId, e.Key })
                .IsUnique(true);
            modelBuilder.Entity<ClientGrantType>()
                .HasIndex(e => new { e.ClientId, e.GrantType })
                .IsUnique(true);
            modelBuilder.Entity<ClientIdpRestriction>()
                .HasIndex(e => new { e.ClientId, e.Provider })
                .IsUnique(true);
            modelBuilder.Entity<ApiClaim>()
                .HasIndex(e => new { e.ApiId, e.Type })
                .IsUnique(true);
            modelBuilder.Entity<ApiScope>()
                .HasIndex(e => new { e.ApiId, e.Scope })
                .IsUnique(true);
            modelBuilder.Entity<ApiProperty>()
                .HasIndex(e => new { e.ApiId, e.Key })
                .IsUnique(true);
            modelBuilder.Entity<ApiScopeClaim>()
                .HasIndex(e => new { e.ApiScpopeId, e.Type })
                .IsUnique(true);
            modelBuilder.Entity<IdentityProperty>()
                .HasIndex(e => new { e.IdentityId, e.Key })
                .IsUnique(true);
            modelBuilder.Entity<IdentityClaim>()
                .HasIndex(e => new { e.IdentityId, e.Type })
                .IsUnique(true);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            SetAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetAuditFields();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void SetAuditFields()
        {
            var entryList = ChangeTracker.Entries<IAuditable>();
            foreach (var entry in entryList)
            {
                var entity = entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.ModifiedAt = null;
                }
                if (entry.State == EntityState.Modified)
                {
                    entity.ModifiedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
