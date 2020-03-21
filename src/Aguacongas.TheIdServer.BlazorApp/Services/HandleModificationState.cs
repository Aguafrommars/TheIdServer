using Aguacongas.IdentityServer.Store.Entity;
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
        public Action OnStateChange { get; set; }
        public Dictionary<Type, Dictionary<object, ModificationKind>> Changes { get; } = new Dictionary<Type, Dictionary<object, ModificationKind>>();

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
                Console.WriteLine($"Replace change for entity {entityType.Name} {modification} with {ModificationKind.Add}");
                modifications[entity] = ModificationKind.Add;
                OnStateChange?.Invoke();
                return;
            }
            OnStateChange?.Invoke();
            Console.WriteLine($"Add created change for entity {entityType.Name}");
        }

        public void EntityDeleted<TEntity>(TEntity entity) where TEntity: IEntityId
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            var entityType = typeof(TEntity);
            var modifications = GetModifications(entityType);
            if (entity.Id == null)
            {
                Console.WriteLine($"Remove change for entity {entityType.Name} {entity.Id}");
                modifications.Remove(entity);
                OnStateChange?.Invoke();
                return;
            }
            Console.WriteLine($"Add delete change for entity {entityType.Name} {entity.Id}");
            modifications.Add(entity, ModificationKind.Delete);
            OnStateChange?.Invoke();
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
                Console.WriteLine($"Add update modification for entity {entityType}");
                modifications.Add(entity, ModificationKind.Update);
            }
        }
    }
}
