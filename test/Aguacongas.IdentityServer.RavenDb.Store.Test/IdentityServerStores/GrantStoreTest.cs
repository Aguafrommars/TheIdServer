// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityServerStores
{
    public class GrantStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameter()
        {
            Assert.Throws<ArgumentNullException>(() => new AuthorizationCodeStore(new RavenDbTestDriverWrapper().GetDocumentStore().OpenAsyncSession(),
                null, 
                new Mock<ILogger<AuthorizationCodeStore>>().Object));
        }
    }
}
