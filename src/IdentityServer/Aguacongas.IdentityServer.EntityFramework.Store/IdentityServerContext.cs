using Aguacongas.IdentityServer.Store.Entitiy;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityServerContext<TKey>:DbContext where TKey:IEquatable<TKey>
    {
        public DbSet<Api<TKey>> Apis { get; set; }

        public DbSet<ApiClaim<TKey>> ApiClaims { get; set; }

        public DbSet<ApiScope<TKey>> ApiScopes { get; set; }

        public DbSet<ApiScopeClaim<TKey>> ApiScopeClaims { get; set; }

        public DbSet<ApiSecret<TKey>> ApiSecrets { get; set; }

        public DbSet<ClaimType<TKey>> ClaimTypes { get; set; }

        public DbSet<Client<TKey>> Clients { get; set; }

        public DbSet<ClientClaim<TKey>> ClientClaims { get; set; }

        public DbSet<ClientCorsOrigin<TKey>> ClientCorsOrigins { get; set; }

        public DbSet<ClientGrantType<TKey>> ClientGrantTypes { get; set; }

        public DbSet<ClientPostLogoutRedirectUri<TKey>> ClientPostLogoutRedirectUris { get; set; }

        public DbSet<ClientProperty<TKey>> ClientProperties { get; set; }

        public DbSet<ClientRedirectUri<TKey>> ClientRedirectUris { get; set; }

        public DbSet<ClientScope<TKey>> ClientScopes { get; set; }

        public DbSet<ClientSecret<TKey>> ClientSecrets { get; set; }

        public DbSet<DeviceCode<TKey>> DeviceCodes { get; set; }

        public DbSet<Identity<TKey>> Identities { get; set; }

        public DbSet<IdentityClaim<TKey>> IdentityClaims { get; set; }

        public DbSet<IdentityProperty<TKey>> IdentityPropertys { get; set; }

        public DbSet<ReferenceToken<TKey>> ReferenceTokens { get; set; }

        public DbSet<RefreshToken<TKey>> RefreshTokens { get; set; }

        public DbSet<UserConstent<TKey>> UserConstents { get; set; }
    }
}
