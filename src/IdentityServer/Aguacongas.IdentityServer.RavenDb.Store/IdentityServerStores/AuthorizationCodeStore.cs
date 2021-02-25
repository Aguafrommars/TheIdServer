// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class AuthorizationCodeStore : GrantStore<Entity.AuthorizationCode, AuthorizationCode>, IAuthorizationCodeStore
    {

        public AuthorizationCodeStore(IAsyncDocumentSession session, 
            IPersistentGrantSerializer serializer,
            ILogger<AuthorizationCodeStore> logger)
            : base(session, serializer, logger)
        {
        }

        public Task<AuthorizationCode> GetAuthorizationCodeAsync(string code)
            => GetAsync(code);

        public Task RemoveAuthorizationCodeAsync(string code)
            => RemoveAsync(code);

        public Task<string> StoreAuthorizationCodeAsync(AuthorizationCode code)
            => StoreAsync(code, code.CreationTime.AddSeconds(code.Lifetime));

        protected override string GetClientId(AuthorizationCode dto)
            => dto?.ClientId;

        protected override string GetSubjectId(AuthorizationCode dto)
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
            

        protected override DateTime? GetExpiration(AuthorizationCode dto)
            => dto.CreationTime.AddSeconds(dto.Lifetime);
    }
}
