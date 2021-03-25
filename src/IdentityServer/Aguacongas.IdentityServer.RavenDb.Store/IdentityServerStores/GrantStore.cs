// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public abstract class GrantStore<TEntity, TDto> : AdminStore<TEntity>
        where TEntity: class, IGrant, new()
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IPersistentGrantSerializer _serializer;

        protected GrantStore(ScopedAsynDocumentcSession session, IPersistentGrantSerializer serializer, ILogger<GrantStore<TEntity, TDto>> logger)
            : base(session, logger)
        {
            _session = session.Session;
            _serializer = serializer ?? throw new ArgumentNullException(nameof(session));
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

            await _session.SaveChangesAsync().ConfigureAwait(false);
        }

        protected async Task RemoveAsync(string handle)
        {
            var entity = await GetEntityByHandle(handle)
                .ConfigureAwait(false);
            if (entity == null)
            {
                return;
            }
            _session.Delete(entity);
            await _session.SaveChangesAsync().ConfigureAwait(false);
        }

        protected async Task RemoveAsync(string subjectId, string clientId)
        {
            var entity = await GetEntityBySubjectAndClient(subjectId, clientId)
                .ConfigureAwait(false);

            await RemoveEntityAsync(entity)
                .ConfigureAwait(false);
        }

        private async Task RemoveEntityAsync(TEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            try
            {
                _session.Delete(entity);
                await _session.SaveChangesAsync().ConfigureAwait(false);
            }
            catch
            {
                // remove can already be done
            }
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
                await _session.StoreAsync(entity, $"{typeof(TEntity).Name.ToLowerInvariant()}/{entity.Id}");
            }
            else
            {
                entity.Data = _serializer.Serialize(dto);
                entity.Expiration = GetExpiration(dto);
            }

            await _session.SaveChangesAsync().ConfigureAwait(false);
            return entity.Id;
        }

        protected abstract DateTime? GetExpiration(TDto dto);

        protected abstract string GetClientId(TDto dto);

        protected abstract string GetSubjectId(TDto dto);

        protected virtual async Task<TEntity> GetEntityByHandle(string handle)
        {
            return await _session.Query<TEntity>()
                .FirstOrDefaultAsync(t => t.Id == handle)
                .ConfigureAwait(false);
        }

        protected virtual async Task<TEntity> GetEntityBySubjectAndClient(string subjectId, string clientId)
        {
            return await _session.Query<TEntity>()
                .FirstOrDefaultAsync(c => c.UserId == subjectId && c.ClientId == clientId)
                .ConfigureAwait(false);
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
