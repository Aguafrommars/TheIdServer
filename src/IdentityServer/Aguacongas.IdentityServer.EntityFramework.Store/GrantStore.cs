using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public abstract class GrantStore<TEntity, TDto> : AdminStore<TEntity, OperationalDbContext>
        where TEntity: class, IGrant, new()
    {
        private readonly OperationalDbContext _context;
        private readonly IPersistentGrantSerializer _serializer;

        protected GrantStore(OperationalDbContext context, IPersistentGrantSerializer serializer, ILogger<GrantStore<TEntity, TDto>> logger)
            : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(context));
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

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        protected async Task RemoveAsync(string handle)
        {
            var entity = await GetEntityByHandle(handle)
                .ConfigureAwait(false);

            _context.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
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
                _context.Remove(entity);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
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
                await _context.AddAsync(entity);
            }
            else
            {
                entity.Data = _serializer.Serialize(dto);
                entity.Expiration = GetExpiration(dto);
            }

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException e) when (e.InnerException == null)
            {
                // store can already be done
            }
            return entity.Id;
        }

        protected abstract DateTime? GetExpiration(TDto dto);

        protected abstract string GetClientId(TDto dto);

        protected abstract string GetSubjectId(TDto dto);

        protected virtual async Task<TEntity> GetEntityByHandle(string handle)
        {
            return await _context.Set<TEntity>()
                .FirstOrDefaultAsync(t => t.Id == handle)
                .ConfigureAwait(false);
        }

        protected virtual async Task<TEntity> GetEntityBySubjectAndClient(string subjectId, string clientId)
        {
            return await _context.Set<TEntity>()
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
