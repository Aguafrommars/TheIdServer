using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ConfigurationDbContext : DbContext
    {
        public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options):base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<ClientClaim> ClientClaims { get; set; }

        public virtual DbSet<ClientGrantType> ClientGrantTypes { get; set; }

        public virtual DbSet<ClientProperty> ClientProperties { get; set; }

        public virtual DbSet<ClientUri> ClientUris { get; set; }

        public virtual DbSet<ClientScope> ClientScopes { get; set; }

        public virtual DbSet<ClientSecret> ClientSecrets { get; set; }

        public virtual DbSet<ProtectResource> Apis { get; set; }

        public virtual DbSet<ApiClaim> ApiClaims { get; set; }

        public virtual DbSet<ApiScope> ApiScopes { get; set; }

        public virtual DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        public virtual DbSet<ApiSecret> ApiSecrets { get; set; }

        public virtual DbSet<IdentityResource> Identities { get; set; }

        public virtual DbSet<IdentityClaim> IdentityClaims { get; set; }

        public virtual DbSet<IdentityProperty> IdentityProperties { get; set; }

        public virtual DbSet<SchemeDefinition> Providers { get; set; }

        public virtual DbSet<ExternalClaimTransformation> ExternalClaimTransformations { get; set; }

        public virtual DbSet<Culture> Cultures { get; set; }

        public virtual DbSet<LocalizedResource> LocalizedResources { get; set; }

        public virtual DbSet<ApiLocalizedResource> ApiLocalizedResources { get; set; }

        public virtual DbSet<ApiScopeLocalizedResource> ApiScopeLocalizedResources { get; set; }

        public virtual DbSet<ClientLocalizedResource> ClientLocalizedResources { get; set; }

        public virtual DbSet<IdentityLocalizedResource> IdentityLocalizedResources { get; set; }


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
            modelBuilder.Entity<ExternalClaimTransformation>()
                .HasOne<SchemeDefinition>()
                .WithMany(e => e.ClaimTransformations)
                .HasForeignKey(e => e.Scheme);
            modelBuilder.Entity<ExternalClaimTransformation>()
                .HasIndex(e => new { e.Scheme, e.FromClaimType })
                .IsUnique(true);

            modelBuilder.Entity<SchemeDefinition>(b =>
            {
                b.Ignore(p => p.Id)
                  .Ignore(p => p.Options)
                  .Ignore(p => p.HandlerType)
                  .HasKey(p => p.Scheme);
                b.Property(p => p.ConcurrencyStamp).IsConcurrencyToken();
            });

            var defaultCulture = new Culture
            {
                Id = "en-US",
                CreatedAt = DateTime.UtcNow
            };
            modelBuilder.Entity<Culture>().HasData(defaultCulture);

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
