using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ClientContext : DbContext
    {
        public ClientContext(DbContextOptions options):base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<ClientClaim> ClientClaims { get; set; }

        public virtual DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }

        public virtual DbSet<ClientGrantType> ClientGrantTypes { get; set; }

        public virtual DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }

        public virtual DbSet<ClientProperty> ClientProperties { get; set; }

        public virtual DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }

        public virtual DbSet<ClientScope> ClientScopes { get; set; }

        public virtual DbSet<ClientSecret> ClientSecrets { get; set; }

        public virtual DbSet<AuthorizationCode> AuthorizationCodes { get; set; }

        public virtual DbSet<ReferenceToken> ReferenceTokens { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<UserConsent> UserConstents { get; set; }
        
        public virtual DbSet<DeviceCode> DeviceCodes { get; set; }

    }
}
