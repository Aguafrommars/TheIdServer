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
        protected T Options => Model.Options as T;

        [CascadingParameter]
        public ExternalProvider Model { get; set; }

        protected override void OnInitialized()
        {
            Model.Options = Model.Options as T ?? Model.DefaultOptions as T;
            base.OnInitialized();
        }


        public override string SerializeOptions()
        {
            return JsonSerializer.Serialize(Options);
        }

    }
}
