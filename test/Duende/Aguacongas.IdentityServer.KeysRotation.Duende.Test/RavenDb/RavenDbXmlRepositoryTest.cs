// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test.RavenDb
{
    public class RavenDbXmlRepositoryTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new RavenDbXmlRepository<DataProtectionKey>(null, null));
            Assert.Throws<ArgumentNullException>(() => new RavenDbXmlRepository<KeyRotationKey>(new Mock<IServiceProvider>().Object, null));
        }

        [Fact]
        public void GetAllElements_should_return_all_elements()
        {
            var sessionMock = new Mock<IDocumentSession>();
            var queryableMock = new Mock<IRavenQueryable<DataProtectionKey>>();
            var fake = new List<DataProtectionKey>
            {
                new DataProtectionKey
                {
                    Xml = "<a/>"
                }
            };
            queryableMock.Setup(m => m.GetEnumerator()).Returns(fake.GetEnumerator());
            sessionMock.Setup(m => m.Query<DataProtectionKey>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(queryableMock.Object);

            var provider = new ServiceCollection()
                .AddLogging()
                .AddTransient(p => new DocumentSessionWrapper(sessionMock.Object))
                .BuildServiceProvider();

            var sut = new RavenDbXmlRepository<DataProtectionKey>(provider, provider.GetRequiredService<ILoggerFactory>());

            var result = sut.GetAllElements();

            Assert.Single(result);
        }

        [Fact]
        public void GetAllElements_should_return_all_elements_whwn_parse_xml_failed()
        {
            var sessionMock = new Mock<IDocumentSession>();
            var queryableMock = new Mock<IRavenQueryable<KeyRotationKey>>();
            var fake = new List<KeyRotationKey>
            {
                new KeyRotationKey
                {

                }
            };
            queryableMock.Setup(m => m.GetEnumerator()).Returns(fake.GetEnumerator());
            sessionMock.Setup(m => m.Query<KeyRotationKey>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(queryableMock.Object);

            var provider = new ServiceCollection()
                .AddLogging()
                .AddTransient(p => new DocumentSessionWrapper(sessionMock.Object))
                .BuildServiceProvider();

            var sut = new RavenDbXmlRepository<KeyRotationKey>(provider, provider.GetRequiredService<ILoggerFactory>());

            var result = sut.GetAllElements();

            Assert.Single(result);
        }

        [Fact]
        public void StoreElement_should_store_element()
        {
            var sessionMock = new Mock<IDocumentSession>();
            sessionMock.Setup(m => m.Store(It.IsAny<object>())).Verifiable();
            sessionMock.Setup(m => m.SaveChanges()).Verifiable();


            var provider = new ServiceCollection()
                .AddLogging()
                .AddTransient(p => new DocumentSessionWrapper(sessionMock.Object))
                .BuildServiceProvider();

            var sut = new RavenDbXmlRepository<KeyRotationKey>(provider, provider.GetRequiredService<ILoggerFactory>());

            sut.StoreElement(XElement.Parse("<a/>"), "test");
            sessionMock.Verify();
        }
    }
}
