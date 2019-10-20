using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System;
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

        public virtual DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }

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

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entryList = ChangeTracker.Entries<IAuditable>();
            foreach (var entry in entryList)
            {
                var entity = entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                if (entry.State == EntityState.Modified)
                {
                    entity.ModifiedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
