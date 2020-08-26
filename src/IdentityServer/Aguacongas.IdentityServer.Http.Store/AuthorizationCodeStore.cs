// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Models = IdentityServer4.Models;

namespace Aguacongas.IdentityServer.Http.Store
{
    public class AuthorizationCodeStore : GrantStore<AuthorizationCode, Models.AuthorizationCode>, IAuthorizationCodeStore
    {
        public AuthorizationCodeStore(IAdminStore<AuthorizationCode> store, 
            IPersistentGrantSerializer serializer) : base(store, serializer)
        {
        }

        public Task<Models.AuthorizationCode> GetAuthorizationCodeAsync(string code)
            => GetAsync(code);

        public Task RemoveAuthorizationCodeAsync(string code)
            => RemoveAsync(code);

        public Task<string> StoreAuthorizationCodeAsync(Models.AuthorizationCode code)
            => StoreAsync(code, code.CreationTime.AddSeconds(code.Lifetime));

        protected override string GetClientId(Models.AuthorizationCode dto)
            => dto?.ClientId;

        protected override string GetSubjectId(Models.AuthorizationCode dto)
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
