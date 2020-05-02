using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
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
        public Models.ExternalProvider ModelBase { get; set; }

        protected override void OnInitialized()
        {
            _wrapper = new ExternalProviderWrapper(ModelBase);
            _wrapper.Options = _wrapper.Options ?? _wrapper.DefaultOptions as T;
            base.OnInitialized();
        }


        public override string SerializeOptions()
        {
            return JsonSerializer.Serialize(Model.Options);
        }

        class ExternalProviderWrapper : IExternalProvider<T>
        {
            private readonly Models.ExternalProvider _parent;

            public ExternalProviderWrapper(Models.ExternalProvider parent)
            {
                _parent = parent;
            }

            public RemoteAuthenticationOptions DefaultOptions => _parent.DefaultOptions;

            public IEnumerable<ExternalProviderKind> Kinds { get => _parent.Kinds; set => _parent.Kinds = value; }
            public T Options { get => (T)_parent.Options; set => _parent.Options = value; }
        }
    }
}
