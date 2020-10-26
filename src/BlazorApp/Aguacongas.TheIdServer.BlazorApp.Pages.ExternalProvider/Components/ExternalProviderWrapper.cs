// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.Components
{
    public class ExternalProviderWrapper<T> : IExternalProvider<T> where T : RemoteAuthenticationOptions
    {
        private readonly Models.ExternalProvider _parent;

        public ExternalProviderWrapper(Models.ExternalProvider parent)
        {
            _parent = parent;
        }

        public T DefaultOptions => (T)_parent.DefaultOptions;

        public IEnumerable<ExternalProviderKind> Kinds { get => _parent.Kinds; set => _parent.Kinds = value; }
        public T Options { get => (T)_parent.Options; set => _parent.Options = value; }
    }
}
