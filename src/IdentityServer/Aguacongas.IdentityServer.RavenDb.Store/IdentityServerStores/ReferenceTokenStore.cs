// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class ReferenceTokenStore : GrantStore<ReferenceToken, Token>, IReferenceTokenStore
    {
        public ReferenceTokenStore(ScopedAsynDocumentcSession session, IPersistentGrantSerializer serializer, ILogger<ReferenceTokenStore> logger)
            : base(session, serializer, logger)
        {
        }

        public Task<Token> GetReferenceTokenAsync(string handle)
            => GetAsync(handle);

        public Task RemoveReferenceTokenAsync(string handle)
            => RemoveAsync(handle);

        public Task RemoveReferenceTokensAsync(string subjectId, string clientId)
            => RemoveAsync(subjectId, clientId);

        public Task<string> StoreReferenceTokenAsync(Token token)
            => StoreAsync(token, token.CreationTime.AddSeconds(token.Lifetime));

        protected override string GetClientId(Token dto) 
            => dto?.ClientId;

        protected override string GetSubjectId(Token dto)
            => dto?.SubjectId;

        protected override DateTime? GetExpiration(Token dto)
            => dto.CreationTime.AddSeconds(dto.Lifetime);
    }
}
