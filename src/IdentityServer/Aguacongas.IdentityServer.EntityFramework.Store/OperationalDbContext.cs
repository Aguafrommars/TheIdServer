// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class OperationalDbContext : DbContext, IDataProtectionKeyContext, IKeyRotationContext
    {
        public OperationalDbContext(DbContextOptions<OperationalDbContext> options):base(options)
        {
        }

        public virtual DbSet<AuthorizationCode> AuthorizationCodes { get; set; }

        public virtual DbSet<ReferenceToken> ReferenceTokens { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<UserConsent> UserConstents { get; set; }
        
        public virtual DbSet<DeviceCode> DeviceCodes { get; set; }

        public virtual DbSet<OneTimeToken> OneTimeTokens { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public DbSet<KeyRotationKey> KeyRotationKeys { get; set; }

        public virtual DbSet<BackChannelAuthenticationRequest> BackChannelAuthenticationRequests { get; set; }

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
