// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test
{
    public class GenericApiControllerTest
    {
        [Fact]
        public void Constructor_should_throw_on_args_null()
        {
            Assert.Throws<ArgumentNullException>(() => new GenericApiController<Key>(null));
        }
    }
}
