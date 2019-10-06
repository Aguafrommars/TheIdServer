using Aguacongas.IdentityServer.Store.Entitiy;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ClientContext: ClientContext<Client>
    {

    }

    public class ClientContext<TClient>: ClientContext<string, TClient>
        where TClient: Client<string>
    {

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
        public DbSet<TClient> Clients { get; set; }

        public DbSet<TClientClaim> ClientClaims { get; set; }

        public DbSet<TClientCorsOrigin> ClientCorsOrigins { get; set; }

        public DbSet<TClientGrantType> ClientGrantTypes { get; set; }

        public DbSet<TClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }

        public DbSet<TClientProperty> ClientProperties { get; set; }

        public DbSet<TClientRedirectUri> ClientRedirectUris { get; set; }

        public DbSet<TClientScope> ClientScopes { get; set; }

        public DbSet<TClientSecret> ClientSecrets { get; set; }

        public DbSet<TReferenceToken> ReferenceTokens { get; set; }

        public DbSet<TRefreshToken> RefreshTokens { get; set; }

        public DbSet<TUserConsent> UserConstents { get; set; }
        
        public DbSet<TDeviceCode> DeviceCodes { get; set; }
    }
}
