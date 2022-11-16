// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
#if DUENDE
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using IsModels = Duende.IdentityServer.Models;
#else
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using IsModels = IdentityServer4.Models;
#endif
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class AuthorizationCodeStore : GrantStore<AuthorizationCode, IsModels.AuthorizationCode>, IAuthorizationCodeStore
    {
        public AuthorizationCodeStore(IAdminStore<AuthorizationCode> store, 
            IPersistentGrantSerializer serializer) : base(store, serializer)
        {
        }

        public Task<IsModels.AuthorizationCode> GetAuthorizationCodeAsync(string code)
            => GetAsync(code);

        public Task RemoveAuthorizationCodeAsync(string code)
            => RemoveAsync(code);

        public Task<string> StoreAuthorizationCodeAsync(IsModels.AuthorizationCode code)
            => StoreAsync(code, code.CreationTime.AddSeconds(code.Lifetime));

        protected override string GetClientId(IsModels.AuthorizationCode dto)
            => dto?.ClientId;

        protected override string GetSubjectId(IsModels.AuthorizationCode dto)
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

        protected override AuthorizationCode CreateEntity(IsModels.AuthorizationCode dto, string clientId, string subjectId, DateTime? expiration)
        {
            var entitiy = base.CreateEntity(dto, clientId, subjectId, expiration);
            entitiy.SessionId = dto.SessionId;
            return entitiy;
        }
    }
}
