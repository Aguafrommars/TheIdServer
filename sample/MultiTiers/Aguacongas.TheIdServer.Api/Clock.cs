// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Duende.IdentityServer;
using System;

namespace Aguacongas.TheIdServer.Api;

internal class Clock : IClock
{
    public DateTimeOffset UtcNow => DateTime.UtcNow;
}