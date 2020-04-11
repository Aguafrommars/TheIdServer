using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Components.ExternalProviderComponents;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class ExternalProvider
    {
        private ProviderOptionsBase _optionsComponent;

        protected override string Expand => "Secrets,Scopes,Scopes/ApiScopeClaims,ApiClaims,Properties";

        protected override bool NonEditable => false;

        protected override string BackUrl => "providers";

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            // no nav
        }

        protected override Models.ExternalProvider Create()
        {
            return new Models.ExternalProvider();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            var providerKindsResponse = await _providerKindStore.GetAsync(new PageRequest()).ConfigureAwait(false);
            Model.Kinds = providerKindsResponse.Items;
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is Models.ExternalProvider provider)
            {
                provider.SerializedOptions = _optionsComponent.SerializeOptions();
            }
        }
    }
}
