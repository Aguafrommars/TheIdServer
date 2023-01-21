// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System;
using System.Reflection;

namespace MongoDB.Driver
{
    public static class MongoDatabaseExtension
    {
        static MethodInfo _getCollectionMethod = typeof(IMongoDatabase).GetMethod(nameof(IMongoDatabase.GetCollection));
        static MongoCollectionSettings _settings = new MongoCollectionSettings
        {
            AssignIdOnInsert = false
        };

        public static object GetCollection(this IMongoDatabase database, Type entityType)
        {
            var genericGetCollectionMethod = _getCollectionMethod.MakeGenericMethod(entityType);
            return genericGetCollectionMethod.Invoke(database, new object [] { entityType.Name, _settings });
        }
    }
}
