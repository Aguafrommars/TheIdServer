// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IRetrieveOneTimeToken
    {
        string GetOneTimeToken(string id);
    }
}
