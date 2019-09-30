using Aguacongas.IdentityServer.Store.Entitiy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
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
        TDeviceCode,
        TClaimType> :ClaimTypeContext<TKey, TClaimType>
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
        where TClaimType : ClaimType<TKey>
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
