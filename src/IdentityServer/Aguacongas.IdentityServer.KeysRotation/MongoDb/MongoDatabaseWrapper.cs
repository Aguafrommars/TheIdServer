// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Aguacongas.IdentityServer.KeysRotation.MongoDb
{
    public class MongoCollectionWrapper<TKey>
        where TKey : IXmlKey
    {
        public IMongoCollection<TKey> Collection { get; }
        public IMongoQueryable<TKey> Queryable { get; }


        public MongoCollectionWrapper(IMongoCollection<TKey> collection)
            :this(collection, collection.AsQueryable())
        {
        }

        public MongoCollectionWrapper(IMongoCollection<TKey> collection, IMongoQueryable<TKey> queryable)
        {
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            Queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
        }
    }
}
