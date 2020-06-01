using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Culture
    {
        private readonly GridState _gridState = new GridState();
        private bool _hasMore;
        private CancellationTokenSource _cancellationTokenSource;
        private PageRequest _pageRequest;

        [JSInvokable]
        public Task ScrollBottomReach()
        {
            return LoadMoreAsync();
        }

        protected override string Expand => null;

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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _gridState.OnHeaderClicked += async e =>
            {
                _pageRequest.Skip = 0;
                _pageRequest.OrderBy = e.OrderBy;
                await SetResourcesAsync(Model).ConfigureAwait(false);
                StateHasChanged();
            };

            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                const int resourceHeigth = 160;
                await JSRuntime.InvokeVoidAsync("browserInteropt.onScrollEnd", DotNetObjectReference.Create(this), resourceHeigth);
            }
            base.OnAfterRender(firstRender);
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

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            // nothing to do
        }

        private Task OnFilterChanged(string term)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            return Task.Delay(500, token)
                .ContinueWith(async task =>
                {
                    if (task.IsCanceled)
                    {
                        return;
                    }

                    var propertyArray = new string[]
                    {
                        nameof(Entity.LocalizedResource.Key),
                        nameof(Entity.LocalizedResource.Value),
                        nameof(Entity.LocalizedResource.BaseName),
                        nameof(Entity.LocalizedResource.Location),
                    };
                    var expressionArray = new string[propertyArray.Length];
                    for (int i = 0; i < propertyArray.Length; i++)
                    {
                        expressionArray[i] = $"contains({propertyArray[i]},'{term}')";
                    }
                    _pageRequest.Skip = 0;
                    _pageRequest.Filter = $"{nameof(Entity.LocalizedResource.CultureId)} eq '{Model.Id}' and ({string.Join(" or ", expressionArray)})";

                    await SetResourcesAsync(Model).ConfigureAwait(false);

                    await InvokeAsync(() => StateHasChanged())
                        .ConfigureAwait(false);
                }, TaskScheduler.Default);
        }

        protected override async Task<Entity.Culture> GetModelAsync()
        {
            if (Model != null)
            {
                return Model;
            }
            var entity = await base.GetModelAsync().ConfigureAwait(false);
            _pageRequest = new PageRequest
            {
                Filter = $"{nameof(Entity.LocalizedResource.CultureId)} eq '{entity.Id}'",
                OrderBy = nameof(Entity.LocalizedResource.Key),
                Skip = 0,
                Take = 10
            };
            await SetResourcesAsync(entity).ConfigureAwait(false);
            return entity;
        }

        protected override void OnStateChange(ModificationKind kind, object entity)
        {
            switch (kind)
            {
                case ModificationKind.Add:
                    State.Resources.Add(entity as Entity.LocalizedResource);
                    break;
                case ModificationKind.Delete:
                    State.Resources.Remove(entity as Entity.LocalizedResource);
                    break;
            }
            base.OnStateChange(kind, entity);
        }

        private async Task SetResourcesAsync(Entity.Culture model)
        {
            var page = await _localizedResourceStore.GetAsync(_pageRequest).ConfigureAwait(false);
            model.Resources = page.Items.ToList();
            _hasMore = model.Resources.Count < page.Count;
        }

        private Entity.LocalizedResource CreateResource()
            => new Entity.LocalizedResource();

        private void OnDeleteResourceClicked(Entity.LocalizedResource resource)
        {
            Model.Resources.Remove(resource);
            EntityDeleted(resource);
        }

        private async Task LoadMoreAsync()
        {
            if (!_hasMore)
            {
                return;
            }
            _pageRequest.Skip += _pageRequest.Take;
            var page = await _localizedResourceStore.GetAsync(_pageRequest).ConfigureAwait(false);
            var resourceList = Model.Resources;
            foreach (var resource in page.Items)
            {
                resourceList.Add(resource);
            }
            _hasMore = resourceList.Count < page.Count;
            StateHasChanged();
        }
    }
}
