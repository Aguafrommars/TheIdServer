using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores.Serialization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public abstract class GrantStore<TEntity, TDto> 
        where TEntity: class, IGrant, new()
    {
        private readonly IdentityServerDbContext _context;
        private readonly IPersistentGrantSerializer _serializer;

        public GrantStore(IdentityServerDbContext context, IPersistentGrantSerializer serializer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(context));
        }

        protected async Task<TDto> GetAsync(string handle)
        {
            var entity = await GetEntityByHandle(handle)
                .ConfigureAwait(false);

            return CreateDto(entity.Data);
        }

        protected async Task<TDto> GetAsync(string subjectId, string clientId)
        {
            var entity = await GetEntityBySubjectAndClient(subjectId, clientId)
                .ConfigureAwait(false);

            return CreateDto(entity.Data);
        }

        protected async Task UpdateAsync(string handle, TDto dto)
        {
            var entity = await GetEntityByHandle(handle)
                .ConfigureAwait(false);

            if (entity == null)
            {
                throw new InvalidOperationException($"{dto.GetType().Name} {handle} not found");
            }

            var subjectId = GetSubjectId(dto);
            var clientId = GetClientId(dto);
            var client = await GetClientAsync(clientId)
                .ConfigureAwait(false);

            var newEntity = CreateEntity(dto, client, subjectId);
            entity.Data = newEntity.Data;

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        protected async Task RemoveAsync(string handle)
        {
            var entity = await GetEntityByHandle(handle)
                .ConfigureAwait(false);

            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        protected async Task RemoveAsync(string subjectId, string clientId)
        {
            var entity = await GetEntityBySubjectAndClient(subjectId, clientId)
                .ConfigureAwait(false);

            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        protected async Task<string> StoreAsync(TDto dto)
        {
            dto = dto ?? throw new ArgumentNullException(nameof(dto));

            var clientId = GetClientId(dto);
            var client = await GetClientAsync(clientId)
                .ConfigureAwait(false);

            var subjectId = GetSubjectId(dto);

            var entity = await GetEntityBySubjectAndClient(subjectId, clientId)
                .ConfigureAwait(false);

            if (entity == null)
            {
                entity = CreateEntity(dto, client, subjectId);
                await _context.AddAsync(entity);
            }
            else
            {
                entity.Data = _serializer.Serialize(dto);
            }
            
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity.Id;
        }

        protected virtual async Task<Client> GetClientAsync(string clientId)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException($"Client {clientId} not found");
            }

            return client;
        }

        protected abstract string GetClientId(TDto dto);

        protected abstract string GetSubjectId(TDto dto);

        protected virtual async Task<TEntity> GetEntityByHandle(string handle)
        {
            return await _context
                .FindAsync<TEntity>(handle)
                .ConfigureAwait(false);
        }

        protected virtual async Task<TEntity> GetEntityBySubjectAndClient(string subjectId, string clientId)
        {
            return await _context.Set<TEntity>()
                .FirstOrDefaultAsync(c => c.SubjectId == subjectId & c.Client.Id == clientId)
                .ConfigureAwait(false);
        }

        protected virtual TEntity CreateEntity(TDto dto, Client client, string subjectId)
        {
            return new TEntity
            {
                Id = Guid.NewGuid().ToString(),
                SubjectId = subjectId,
                Client = client,
                Data = _serializer.Serialize(dto)
            };
        }

        protected virtual TDto CreateDto(string data)
        {
            return _serializer.Deserialize<TDto>(data);
        }
    }
}
