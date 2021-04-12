// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public abstract class GrantStore<TEntity, TDto>
        where TEntity: class, IGrant, new()
    {
        private readonly IAdminStore<TEntity> _store;
        private readonly IPersistentGrantSerializer _serializer;

        protected GrantStore(IAdminStore<TEntity> store, IPersistentGrantSerializer serializer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        protected async Task<TDto> GetAsync(string handle)
        {
            var entity = await GetEntityByHandle(handle)
                .ConfigureAwait(false);

            return CreateDto(entity?.Data);
        }

        protected async Task<TDto> GetAsync(string subjectId, string clientId)
        {
            var entity = await GetEntityBySubjectAndClient(subjectId, clientId)
                .ConfigureAwait(false);

            return CreateDto(entity?.Data);
        }

        protected async Task UpdateAsync(string handle, TDto dto, DateTime? expiration)
        {
            var entity = await GetEntityByHandle(handle)
                .ConfigureAwait(false);

            if (entity == null)
            {
                throw new InvalidOperationException($"{dto.GetType().Name} {handle} not found");
            }

            var subjectId = GetSubjectId(dto);
            var clientId = GetClientId(dto); 

            var newEntity = CreateEntity(dto, clientId, subjectId, expiration);
            entity.Data = newEntity.Data;

            await _store.UpdateAsync(entity).ConfigureAwait(false);
        }

        protected async Task RemoveAsync(string handle)
        {
            var entity = await GetEntityByHandle(handle)
                .ConfigureAwait(false);

            await _store.DeleteAsync(entity.Id)
                .ConfigureAwait(false);
        }

        protected async Task RemoveAsync(string subjectId, string clientId)
        {
            var entity = await GetEntityBySubjectAndClient(subjectId, clientId)
                .ConfigureAwait(false);

            await RemoveEntityAsync(entity)
                .ConfigureAwait(false);
        }

        private Task RemoveEntityAsync(TEntity entity)
        {
            if (entity == null)
            {
                return Task.FromResult<TEntity>(null);
            }

            return _store.DeleteAsync(entity.Id);
        }

        protected async Task<string> StoreAsync(TDto dto, DateTime? expiration)
        {
            dto = dto ?? throw new ArgumentNullException(nameof(dto));

            var clientId = GetClientId(dto);

            var subjectId = GetSubjectId(dto);

            var entity = await GetEntityBySubjectAndClient(subjectId, clientId)
                .ConfigureAwait(false);

            if (entity == null)
            {
                entity = CreateEntity(dto, clientId, subjectId, expiration);
                entity = await _store.CreateAsync(entity).ConfigureAwait(false);
            }
            else
            {
                entity.Data = _serializer.Serialize(dto);
                await _store.UpdateAsync(entity).ConfigureAwait(false);
            }

            return entity.Id;
        }

        protected abstract string GetClientId(TDto dto);

        protected abstract string GetSubjectId(TDto dto);

        protected virtual Task<TEntity> GetEntityByHandle(string handle)
        {
            return _store.GetAsync(handle, null);
        }

        protected virtual async Task<TEntity> GetEntityBySubjectAndClient(string subjectId, string clientId)
        {
            return (await _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserConsent.UserId)} eq '{subjectId}' And {nameof(UserConsent.ClientId)} eq '{clientId}'"
            }).ConfigureAwait(false)).Items.FirstOrDefault();
        }

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Cannot be null")]
        protected virtual TEntity CreateEntity(TDto dto, string clientId, string subjectId, DateTime? expiration)
        {
            return new TEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = subjectId,
                ClientId = clientId,
                Data = _serializer.Serialize(dto),
                Expiration = expiration
            };
        }

        protected virtual TDto CreateDto(string data)
        {            
            if (data == null)
            {
                return default;
            }
            return _serializer.Deserialize<TDto>(data);
        }
    }
}
