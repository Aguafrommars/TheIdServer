// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ExternalProviderComponents
{
    public abstract class ProviderOptionsBase : ComponentBase
    {
        public abstract string SerializeOptions();
    }

    public abstract class ProviderOptionsBase<T> : ProviderOptionsBase where T: RemoteAuthenticationOptions
    {
        private ExternalProviderWrapper _wrapper;
        protected IExternalProvider<T> Model => _wrapper;

        [CascadingParameter]
        public ExternalProvider ModelBase { get; set; }

        [Inject]
        protected IStringLocalizerAsync<ProviderOptionsBase<T>> Localizer { get; set; }

        protected override void OnInitialized()
        {
            _wrapper = new ExternalProviderWrapper(ModelBase);
            _wrapper.Options ??= _wrapper.DefaultOptions;
            base.OnInitialized();
        }


        public override string SerializeOptions()
        {
            return JsonSerializer.Serialize(Model.Options);
        }

        private class ExternalProviderWrapper : ExternalProviderWrapper<T>
        {
            public ExternalProviderWrapper(ExternalProvider parent)
                : base(parent)
            {

            }
        }
    }
}
