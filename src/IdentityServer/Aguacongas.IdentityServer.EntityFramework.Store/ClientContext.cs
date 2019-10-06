using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ClientContext: ClientContext<Client>
    {
        public ClientContext(DbContextOptions options) : base(options)
        {

        }
    }

    public class ClientContext<TClient>: ClientContext<string, TClient>
        where TClient: Client<string>
    {
        public ClientContext(DbContextOptions options) : base(options)
        {

        }
    }

    public class ClientContext<TKey,
        TClient>: ClientContext<TKey, 
        TClient,
        ClientClaim<TKey>,
        ClientCorsOrigin<TKey>,
        ClientGrantType<TKey>,
        ClientPostLogoutRedirectUri<TKey>,
        ClientRedirectUri<TKey>,
        ClientProperty<TKey>,
        ClientScope<TKey>,
        ClientSecret<TKey>,
        ReferenceToken<TKey>,
        RefreshToken<TKey>,
        UserConsent<TKey>,
        DeviceCode<TKey>>
        where TKey: IEquatable<TKey>
        where TClient: Client<TKey>
    {
        public ClientContext(DbContextOptions options) : base(options)
        {

        }
    }

    public class ClientContext<TKey,
        TClient,
        TClientClaim,
        TClientCorsOrigin,
        TClientGrantType,
        TClientPostLogoutRedirectUri,
        TClientRedirectUri,
        TClientProperty,
        TClientScope,
        TClientSecret,
        TReferenceToken,
        TRefreshToken,
        TUserConsent,
        TDeviceCode>: DbContext
        where TKey: IEquatable<TKey>
        where TClient : Client<TKey>
        where TClientClaim : ClientClaim<TKey>
        where TClientCorsOrigin : ClientCorsOrigin<TKey>
        where TClientGrantType : ClientGrantType<TKey>
        where TClientPostLogoutRedirectUri : ClientPostLogoutRedirectUri<TKey>
        where TClientRedirectUri : ClientRedirectUri<TKey>
        where TClientProperty : ClientProperty<TKey>
        where TClientScope : ClientScope<TKey>
        where TClientSecret : ClientSecret<TKey>
        where TReferenceToken : ReferenceToken<TKey>
        where TRefreshToken : RefreshToken<TKey>
        where TUserConsent : UserConsent<TKey>
        where TDeviceCode : DeviceCode<TKey>
    {
        public ClientContext(DbContextOptions options):base(options)
        {

        }
        public virtual DbSet<TClient> Clients { get; set; }

        public virtual DbSet<TClientClaim> ClientClaims { get; set; }

        public virtual DbSet<TClientCorsOrigin> ClientCorsOrigins { get; set; }

        public virtual DbSet<TClientGrantType> ClientGrantTypes { get; set; }

        public virtual DbSet<TClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }

        public virtual DbSet<TClientProperty> ClientProperties { get; set; }

        public virtual DbSet<TClientRedirectUri> ClientRedirectUris { get; set; }

        public virtual DbSet<TClientScope> ClientScopes { get; set; }

        public virtual DbSet<TClientSecret> ClientSecrets { get; set; }

        public virtual DbSet<TReferenceToken> ReferenceTokens { get; set; }

        public virtual DbSet<TRefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<TUserConsent> UserConstents { get; set; }
        
        public virtual DbSet<TDeviceCode> DeviceCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
            modelBuilder.Entity<Client>()
                .HasIndex(m => m.ClientId)
                .IsUnique(true);
        }
    }
}
