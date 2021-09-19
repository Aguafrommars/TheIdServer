﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using System;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test
{
    public class GenericKeyControllerTest
    {
        [Fact]
        public void Constructor_should_throw_on_args_null()
        {
            Assert.Throws<ArgumentNullException>(() => new GenericKeyController<IAuthenticatedEncryptorDescriptor>(null));
        }
    }
}
