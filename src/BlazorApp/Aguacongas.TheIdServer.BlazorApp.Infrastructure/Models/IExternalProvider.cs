// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public interface IExternalProvider<TOptions> where TOptions : RemoteAuthenticationOptions
    {
        string Id { get; }
        TOptions DefaultOptions { get; }
        IEnumerable<ExternalProviderKind> Kinds { get; set; }
        TOptions Options { get; set; }
    }
}