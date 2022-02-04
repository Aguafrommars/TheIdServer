// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.IdentityServer.Store
{
    public static class EntityExtensions
    {
        public static void AddEntityId<TEntity>(this ICollection<TEntity> collection, string id) where TEntity: Entity.IEntityId, new()
        {
            collection.Add(new TEntity
            {
                Id = id
            });
        }

        public static void RemoveEntityId<TEntity>(this ICollection<TEntity> collection, string id) where TEntity : Entity.IEntityId
        {
            collection.Remove(collection.First(e => e.Id == id));
        }        
    }
}
