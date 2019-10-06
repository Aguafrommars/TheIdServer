using Aguacongas.IdentityServer.Store.Entitiy;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{

    public class ApiDbContext : ApiDbContext<Api>
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        { }

        protected ApiDbContext() { }
    }

    public class ApiDbContext<TApi>: ApiDbContext<TApi, string>
        where TApi: Api
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        { }

        protected ApiDbContext() { }
    }

    public class ApiDbContext<TApi, TKey> : ApiContext<TKey,
        TApi,
        ApiClaim<TKey>,
        ApiScope<TKey>,
        ApiScopeClaim<TKey>,
        ApiSecret<TKey>>
        where TKey : IEquatable<TKey>
        where TApi : Api<TKey>
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        { }

        protected ApiDbContext() { }
    }

    public abstract class ApiContext<TKey,
        TApi,
        TApiClaim,
        TApiScope,
        TApiScopeClaim,
        TApiSecret>: DbContext
        where TKey:IEquatable<TKey>
        where TApi: Api<TKey>
        where TApiClaim: ApiClaim<TKey>
        where TApiScope: ApiScope<TKey>
        where TApiScopeClaim: ApiScopeClaim<TKey>
        where TApiSecret: ApiSecret<TKey>
    {
        public ApiContext(DbContextOptions options): base(options)
        { }

        protected ApiContext() { }

        public virtual DbSet<TApi> Apis { get; set; }

        public virtual DbSet<TApiClaim> ApiClaims { get; set; }

        public virtual DbSet<TApiScope> ApiScopes { get; set; }

        public virtual DbSet<TApiScopeClaim> ApiScopeClaims { get; set; }

        public virtual DbSet<TApiSecret> ApiSecrets { get; set; }

    }
}
