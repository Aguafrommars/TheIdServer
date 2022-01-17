// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
#if DUENDE
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using models = Duende.IdentityServer.Models;
#else
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using models = IdentityServer4.Models;
#endif
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class AuthorizationCodeStore : GrantStore<AuthorizationCode, models.AuthorizationCode>, IAuthorizationCodeStore
    {
        public AuthorizationCodeStore(IAdminStore<AuthorizationCode> store, 
            IPersistentGrantSerializer serializer) : base(store, serializer)
        {
        }

        public Task<models.AuthorizationCode> GetAuthorizationCodeAsync(string code)
            => GetAsync(code);

        public Task RemoveAuthorizationCodeAsync(string code)
            => RemoveAsync(code);

        public Task<string> StoreAuthorizationCodeAsync(models.AuthorizationCode code)
            => StoreAsync(code, code.CreationTime.AddSeconds(code.Lifetime));

        protected override string GetClientId(models.AuthorizationCode dto)
            => dto?.ClientId;

        protected override string GetSubjectId(models.AuthorizationCode dto)
        {
            var subject = dto?.Subject;
            if (subject == null)
            {
                throw new InvalidOperationException("No subject");
            }

            var idClaim = subject.FindFirst(JwtClaimTypes.Subject) ??
                          subject.FindFirst(ClaimTypes.NameIdentifier) ??
                          subject.FindFirst(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[ClaimTypes.NameIdentifier]) ??
                          throw new Exception("Unknown userid");

            return idClaim.Value;
        }
    }
}
