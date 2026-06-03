// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using IdentityModel;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IsModels = Duende.IdentityServer.Models;

namespace Aguacongas.IdentityServer.Store
{
    public class AuthorizationCodeStore : GrantStore<AuthorizationCode, IsModels.AuthorizationCode>, IAuthorizationCodeStore
    {
        public AuthorizationCodeStore(IAdminStore<AuthorizationCode> store,
            IPersistentGrantSerializer serializer) : base(store, serializer)
        {
        }

        public Task<IsModels.AuthorizationCode> GetAuthorizationCodeAsync(string code, CancellationToken ct)
            => GetAsync(code, ct);

        public Task RemoveAuthorizationCodeAsync(string code, CancellationToken ct)
            => RemoveAsync(code, ct);

        public Task<string> StoreAuthorizationCodeAsync(IsModels.AuthorizationCode code, CancellationToken ct)
            => StoreAsync(code, code.CreationTime.AddSeconds(code.Lifetime), ct);

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
