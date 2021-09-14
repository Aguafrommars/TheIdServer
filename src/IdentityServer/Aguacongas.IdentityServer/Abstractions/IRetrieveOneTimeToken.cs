﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IRetrieveOneTimeToken
    {
        string ConsumeOneTimeToken(string id);

        string GetOneTimeToken(string id);
    }
}
