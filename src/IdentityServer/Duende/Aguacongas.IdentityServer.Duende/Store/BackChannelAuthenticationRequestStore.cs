using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsModels = Duende.IdentityServer.Models;

namespace Aguacongas.IdentityServer.Store
{
    public class BackChannelAuthenticationRequestStore : GrantStore<BackChannelAuthenticationRequest, IsModels.BackChannelAuthenticationRequest>, IBackChannelAuthenticationRequestStore
    {
        private readonly IAdminStore<BackChannelAuthenticationRequest> _store;

        public BackChannelAuthenticationRequestStore(IAdminStore<BackChannelAuthenticationRequest> store, IPersistentGrantSerializer serializer) : base(store, serializer)
        {
            _store = store;
        }

        public async Task<string> CreateRequestAsync(IsModels.BackChannelAuthenticationRequest request)
        {
            var clientId = GetClientId(request);
            var subjectId = GetSubjectId(request);
            request.InternalId ??= Guid.NewGuid().ToString();

            var entity = CreateEntity(request, clientId, subjectId, request.CreationTime.AddSeconds(request.Lifetime));
            entity = await _store.CreateAsync(entity).ConfigureAwait(false);

            return entity.SessionId;
        }

        public async Task<IsModels.BackChannelAuthenticationRequest> GetByAuthenticationRequestIdAsync(string requestId)
        {
            var page = await _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(BackChannelAuthenticationRequest.SessionId)} eq '{requestId}'"
            }).ConfigureAwait(false);

            return CreateDto(page.Items.FirstOrDefault()?.Data);
        }

        public Task<IsModels.BackChannelAuthenticationRequest> GetByInternalIdAsync(string id)
        => GetAsync(id);
        

        public async Task<IEnumerable<IsModels.BackChannelAuthenticationRequest>> GetLoginsForUserAsync(string subjectId, string clientId = null)
        {
            var filter = $"{nameof(BackChannelAuthenticationRequest.UserId)} eq '{subjectId}'";
            if (clientId is not null)
            {
                filter += $" And {nameof(BackChannelAuthenticationRequest.UserId)} eq '{clientId}'";
            }

            var page = await _store.GetAsync(new PageRequest
            {
                Filter = filter
            }).ConfigureAwait(false);

            return page.Items.Select(e => CreateDto(e.Data));
        }

        public Task RemoveByInternalIdAsync(string id)
        => RemoveAsync(id);

        public Task UpdateByInternalIdAsync(string id, IsModels.BackChannelAuthenticationRequest request)
        => UpdateAsync(id, request, request.CreationTime.AddSeconds(request.Lifetime));

        protected override string GetClientId(IsModels.BackChannelAuthenticationRequest dto)
        => dto.ClientId;

        protected override string GetSubjectId(IsModels.BackChannelAuthenticationRequest dto)
        => dto.Subject.FindFirst(c => c.Type == JwtClaimTypes.Subject)?.Value;

        protected override BackChannelAuthenticationRequest CreateEntity(IsModels.BackChannelAuthenticationRequest dto, string clientId, string subjectId, DateTime? expiration)
        {
            var entity = base.CreateEntity(dto, clientId, subjectId, expiration);            
            entity.Id = dto.InternalId;
            entity.SessionId = dto.SessionId ?? Guid.NewGuid().ToString();
            return entity;
        }
        
    }
}
