using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Culture
    {

        private CultureInfo Info => CultureInfo.GetCultureInfo(Id);


        protected override string Expand => nameof(Entity.Culture.Resources);

        protected override bool NonEditable => false;

        protected override string BackUrl => "cultures";

        protected override async Task<Entity.Culture> Create()
        {
            var resourceStore = Provider.GetRequiredService<IAdminStore<Entity.LocalizedResource>>();
            var responses = await resourceStore.GetAsync(new PageRequest
            {
                Select = nameof(Entity.LocalizedResource.Key),
                Filter = $"{nameof(Entity.LocalizedResource.CultureId)} eq 'en-US'"
            }).ConfigureAwait(false);

            foreach(var item in responses.Items)
            {
                item.Id = Guid.NewGuid().ToString();
                EntityCreated(item);
            }

            return new Entity.Culture
            {
                Resources = responses.Items.ToList()
            };
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            HandleModificationState.OnFilterChange += HandleModificationState_OnFilterChange;
            HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
        }

        private void HandleModificationState_OnStateChange(ModificationKind kind, object entity)
        {
            StateHasChanged();
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is Entity.LocalizedResource resource)
            {
                if (string.IsNullOrEmpty(resource.Id))
                {
                    resource.Id = Guid.NewGuid().ToString();
                }
                resource.CultureId = Model.Id;
            }
        }

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is Entity.Culture culture)
            {
                culture.Resources = null;
            }
        }

        private void HandleModificationState_OnFilterChange(string term)
        {
            StateHasChanged();
        }

        private Entity.LocalizedResource CreateResource()
            => new Entity.LocalizedResource();
    }
}
