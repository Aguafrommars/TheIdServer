﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IProviderClient
    {
        Task ProviderAddedAsync(string scheme, CancellationToken cancellationToken = default);

        Task ProviderUpdatedAsync(string scheme, CancellationToken cancellationToken = default);

        Task ProviderRemovedAsync(string scheme, CancellationToken cancellationToken = default);

        Task KeyRevokedAsync(string kind, string id, CancellationToken cancellationToken = default);
    }
}
