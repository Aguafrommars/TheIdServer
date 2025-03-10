﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test.MongoDb
{
    public class MongoDbXmlRepositoryTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new MongoDbXmlRepository<DataProtectionKey>(null, null));
            Assert.Throws<ArgumentNullException>(() => new MongoDbXmlRepository<KeyRotationKey>(new Mock<IServiceProvider>().Object, null));
        }

        [Fact]
        public void GetAllElements_should_return_all_elements()
        {
            var collectionMock = new Mock<IMongoCollection<DataProtectionKey>>();
            var queryableMock = new Mock<IQueryable<DataProtectionKey>>();
            var fake = new List<DataProtectionKey>
            {
                new DataProtectionKey
                {
                    Xml = "<a/>"
                }
            };
            queryableMock.Setup(m => m.GetEnumerator()).Returns(fake.GetEnumerator());

            var provider = new ServiceCollection()
                .AddLogging()
                .AddTransient(p => new MongoCollectionWrapper<DataProtectionKey>(collectionMock.Object, queryableMock.Object))
                .BuildServiceProvider();

            var sut = new MongoDbXmlRepository<DataProtectionKey>(provider, provider.GetRequiredService<ILoggerFactory>());

            var result = sut.GetAllElements();

            Assert.Single(result);
        }

        [Fact]
        public void GetAllElements_should_return_all_elements_whwn_parse_xml_failed()
        {
            var collectionMock = new Mock<IMongoCollection<KeyRotationKey>>();
            var queryableMock = new Mock<IQueryable<KeyRotationKey>>();
            var fake = new List<KeyRotationKey>
            {
                new KeyRotationKey
                {

                }
            };
            queryableMock.Setup(m => m.GetEnumerator()).Returns(fake.GetEnumerator());

            var provider = new ServiceCollection()
                .AddLogging()
                .AddTransient(p => new MongoCollectionWrapper<KeyRotationKey>(collectionMock.Object, queryableMock.Object))
                .BuildServiceProvider();

            var sut = new MongoDbXmlRepository<KeyRotationKey>(provider, provider.GetRequiredService<ILoggerFactory>());

            var result = sut.GetAllElements();

            Assert.Single(result);
        }

        [Fact]
        public void StoreElement_should_store_element()
        {
            var clientMock = new Mock<IMongoClient>();
            clientMock.SetupGet(m => m.Settings).Returns(new MongoClientSettings());
            var dbMock = new Mock<IMongoDatabase>();
            dbMock.SetupGet(m => m.Client).Returns(clientMock.Object);
            var collectionMock = new Mock<IMongoCollection<KeyRotationKey>>();
            collectionMock.SetupGet(m => m.Database).Returns(dbMock.Object);
            
            collectionMock.Setup(m => m.InsertOne(It.IsAny<KeyRotationKey>(), null, default)).Verifiable();

            var provider = new ServiceCollection()
                .AddLogging()
                .AddTransient(p => new MongoCollectionWrapper<KeyRotationKey>(collectionMock.Object))
                .BuildServiceProvider();

            var sut = new MongoDbXmlRepository<KeyRotationKey>(provider, provider.GetRequiredService<ILoggerFactory>());

            sut.StoreElement(XElement.Parse("<a/>"), "test");
            collectionMock.Verify();
        }
    }
}
