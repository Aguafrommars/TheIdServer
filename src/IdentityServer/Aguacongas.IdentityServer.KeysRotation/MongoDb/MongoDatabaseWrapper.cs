// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using MongoDB.Driver;

namespace Aguacongas.IdentityServer.KeysRotation.MongoDb
{
    public class MongoCollectionWrapper<TKey>
        where TKey : IXmlKey
    {
        public IMongoCollection<TKey> Collection { get; }

        public MongoCollectionWrapper(IMongoCollection<TKey> collection)
        {
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }
    }
}
