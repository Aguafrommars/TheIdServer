// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class ReferenceTokenStore : GrantStore<ReferenceToken, Token>, IReferenceTokenStore
    {
        private readonly IAdminStore<ReferenceToken> _store;
        public ReferenceTokenStore(IAdminStore<ReferenceToken> store, IPersistentGrantSerializer serializer)
            : base(store, serializer)
        {
            _store = store;
        }

        public Task<Token> GetReferenceTokenAsync(string handle)
            => GetAsync(handle);

        public Task RemoveReferenceTokenAsync(string handle)
            => RemoveAsync(handle);

        public async Task RemoveReferenceTokensAsync(string subjectId, string clientId, string sessionId = null)
        {
            if (sessionId is not null)
            {
                var entity = (await _store.GetAsync(new PageRequest
                {
                    Filter = $"{nameof(ReferenceToken.UserId)} eq '{subjectId}' and {nameof(ReferenceToken.ClientId)} eq '{clientId}' and {nameof(ReferenceToken.SessionId)} eq '{sessionId}'"
                }).ConfigureAwait(false)).Items.FirstOrDefault();

                await RemoveEntityAsync(entity).ConfigureAwait(false);
            }

            await RemoveAsync(subjectId, clientId).ConfigureAwait(false);
        }
            

        public Task<string> StoreReferenceTokenAsync(Token token)
            => StoreAsync(token, token.CreationTime.AddSeconds(token.Lifetime));

        protected override string GetClientId(Token dto) 
            => dto?.ClientId;

        protected override string GetSubjectId(Token dto)
            => dto?.SubjectId;

        protected override ReferenceToken CreateEntity(Token dto, string clientId, string subjectId, DateTime? expiration)
        {
            var entity = base.CreateEntity(dto, clientId, subjectId, expiration);
            entity.SessionId = dto.SessionId;
            return entity;
        }
    }
}
