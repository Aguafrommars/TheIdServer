using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Culture
    {
        private readonly GridState _gridState = new GridState();        

        protected override string Expand => "Resources";

        protected override bool NonEditable => false;

        protected override string BackUrl => "cultures";

        protected override async Task<Entity.Culture> Create()
        {
            var resourceStore = Provider.GetRequiredService<IAdminStore<Entity.LocalizedResource>>();
            var responses = await resourceStore.GetAsync(new PageRequest
            {
                Select = nameof(Entity.LocalizedResource.Key),
                Filter = $"{nameof(Entity.LocalizedResource.Culture)}/{nameof(Entity.Culture.Id)} eq 'en-US'"
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

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var type = typeof(Entity.LocalizedResource);
            _gridState.OnHeaderClicked += e =>
            {
                if (string.IsNullOrEmpty(e.OrderBy))
                {
                    return Task.CompletedTask;
                }
                var segments = e.OrderBy.Split(' ');
                var property = type.GetProperty(segments[0]);
                if (segments.Length > 1)
                {
                    Model.Resources = Model.Resources.OrderByDescending(i => property.GetValue(i)).ToList();
                }
                else
                {
                    Model.Resources = Model.Resources.OrderBy(i => property.GetValue(i)).ToList();
                }

                StateHasChanged();
                return Task.CompletedTask;
            };
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
            if (entity is Entity.Culture culture)
            {
                culture.Resources = null;
            }
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            // nothing to do
        }

        private void OnFilterChanged(string term)
        {
            Model.Resources = State.Resources.Where(r =>
                (r.Key != null && r.Key.Contains(term)) ||
                (r.Value != null && r.Value.Contains(term)) ||
                (r.Location != null && r.Location.Contains(term)) ||
                (r.BaseName != null && r.BaseName.Contains(term)))
                .ToList();

            StateHasChanged();
        }

        private Entity.LocalizedResource CreateResource()
            => new Entity.LocalizedResource();

        private void OnDeleteResourceClicked(Entity.LocalizedResource resource)
        {
            Model.Resources.Remove(resource);
            EntityDeleted(resource);
            StateHasChanged();
        }
    }
}
