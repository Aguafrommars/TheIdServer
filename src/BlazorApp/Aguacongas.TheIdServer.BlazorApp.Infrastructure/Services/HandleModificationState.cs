using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public enum ModificationKind
    {
        Add,
        Update,
        Delete
    }

    public class HandleModificationState
    {
        private readonly ILogger _logger;
        private string _filterTerm = string.Empty;

        public event Action<ModificationKind, object> OnStateChange;

        public event Action<string> OnFilterChange;

        public Dictionary<Type, Dictionary<object, ModificationKind>> Changes { get; } = new Dictionary<Type, Dictionary<object, ModificationKind>>();

        public string FilterTerm 
        {
            get => _filterTerm;
            set
            {
                if (value != _filterTerm)
                {
                    _filterTerm = value ?? string.Empty;
                    OnFilterChange?.Invoke(value);
                }                
            }
        }

        public HandleModificationState(ILogger logger)
        {
            _logger = logger;
        }

        public Dictionary<object, ModificationKind> GetModifications(Type entityType)
        {
            if (!Changes.TryGetValue(entityType, out Dictionary<object, ModificationKind> modifications))
            {
                modifications = new Dictionary<object, ModificationKind>();
                Changes.Add(entityType, modifications);
            }

            return modifications;
        }

        public void EntityCreated<TEntity>(TEntity entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            var entityType = typeof(TEntity);
            var modifications = GetModifications(entityType);
            if (!modifications.TryAdd(entity, ModificationKind.Add))
            {
                var modification = modifications[entity];
                var identifiable = entity as IEntityId;
                _logger.LogDebug($"Replace change for entity {entityType.Name} {identifiable?.Id} {modification} with {ModificationKind.Add}");
                modifications[entity] = ModificationKind.Add;
                OnStateChange?.Invoke(ModificationKind.Add, entity);
                return;
            }
            OnStateChange?.Invoke(ModificationKind.Add, entity);
            _logger.LogDebug($"Add created change for entity {entityType.Name}");
        }

        public void EntityDeleted<TEntity>(TEntity entity) where TEntity: IEntityId
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            var entityType = typeof(TEntity);
            var modifications = GetModifications(entityType);
            if (entity.Id == null)
            {
                _logger.LogDebug($"Remove change for entity {entityType.Name} {entity.Id}");
                modifications.Remove(entity);
                OnStateChange?.Invoke(ModificationKind.Delete, entity);
                return;
            }
            _logger.LogDebug($"Add delete change for entity {entityType.Name} {entity.Id}");
            modifications.Add(entity, ModificationKind.Delete);
            OnStateChange?.Invoke(ModificationKind.Delete, entity);
        }

        public void EntityUpdated<TEntity>(TEntity entity) where TEntity: IEntityId
        {
            var entityType = typeof(TEntity);
            EntityUpdated(entityType, entity);
        }

        public void EntityUpdated(Type entityType, IEntityId entity)
        {
            var modifications = GetModifications(entityType);

            if (!string.IsNullOrEmpty(entity?.Id) && !modifications.ContainsKey(entity))
            {
                _logger.LogDebug($"Add update modification for entity {entityType.Name} {entity.Id}");
                modifications.Add(entity, ModificationKind.Update);
            }
        }
    }
}
