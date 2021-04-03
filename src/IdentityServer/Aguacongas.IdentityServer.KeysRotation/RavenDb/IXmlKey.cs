﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre

namespace Aguacongas.IdentityServer.KeysRotation.RavenDb
{
    public interface IXmlKey
    {
        string Xml { get; set; }
        string FriendlyName { get; set; }
    }
}
