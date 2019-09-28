using Aguacongas.IdentityServer.Store.Entitiy;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ApiContext<TKey,
        TApi,
        TApiClaim,
        TApiScope,
        TApiScopeClaim,
        TApiSecret,
        TClaimType>
        :ClaimTypeContext<TKey, TClaimType> 
        where TKey:IEquatable<TKey>
        where TApi: Api<TKey>
        where TApiClaim : Api<TKey>
        where TApiScope : Api<TKey>
        where TApiScopeClaim : Api<TKey>
        where TApiSecret : Api<TKey>
        where TClaimType : Api<TKey>
    {
        public DbSet<TApi> Apis { get; set; }

        public DbSet<TApiClaim> ApiClaims { get; set; }

        public DbSet<TApiScope> ApiScopes { get; set; }

        public DbSet<TApiScopeClaim> ApiScopeClaims { get; set; }

        public DbSet<TApiSecret> ApiSecrets { get; set; }



        public DbSet<DeviceCode<TKey>> DeviceCodes { get; set; }

        public DbSet<Identity<TKey>> Identities { get; set; }

        public DbSet<IdentityClaim<TKey>> IdentityClaims { get; set; }

        public DbSet<IdentityProperty<TKey>> IdentityPropertys { get; set; }

        public DbSet<ReferenceToken<TKey>> ReferenceTokens { get; set; }

        public DbSet<RefreshToken<TKey>> RefreshTokens { get; set; }

        public DbSet<UserConstent<TKey>> UserConstents { get; set; }
    }
}
