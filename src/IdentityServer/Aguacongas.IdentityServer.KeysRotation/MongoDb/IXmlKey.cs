// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.KeysRotation.MongoDb
{
    public interface IXmlKey
    {
        string Id { get; set; }

        string Xml { get; set; }
        string FriendlyName { get; set; }
    }
}
