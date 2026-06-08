// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Aguacongas.TheIdServer.BlazorApp.BFF.Services;

public class AccessTokenProvider : IAccessTokenProvider
{
    public ValueTask<AccessTokenResult> RequestAccessToken()
    {
        throw new NotImplementedException();
    }

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
    {
        throw new NotImplementedException();
    }
}
