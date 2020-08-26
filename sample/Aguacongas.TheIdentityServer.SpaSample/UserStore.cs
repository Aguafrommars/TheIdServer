// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using System.Security.Claims;

namespace Aguacongas.TheIdentityServer.SpaSample
{
    public class UserStore
    {
        public ClaimsPrincipal User { get; internal set; }
    }
}
