// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    [Authorize(Policy = SharedConstants.READERPOLICY)]
    public abstract class EntitiesModel<T> : ComponentBase, IDisposable where T: class
    {
        private bool _allItemsLoaded;
        private PageRequest _pageRequest;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly List<string> _selectedIdList = new List<string>();

        [Inject]
        protected IAdminStore<T> AdminStore { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IStringLocalizerAsync<EntitiesModel<T>> Localizer { get; set; }

        [Inject]
        protected IAdminStore<OneTimeToken> OneTimeTokenAdminStore { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        protected IEnumerable<T> EntityList { get; private set; } = [];

        protected GridState GridState { get; } = new GridState();

        protected bool ExportDisabled => _selectedIdList.Count == 0;

        protected abstract string SelectProperties { get; }

        protected abstract string ExportExpand { get; }
        protected virtual string Expand { get; }

        [JSInvokable]
        public async Task ScrollBottomReach()
        {
            await GetEntityList(_pageRequest).ConfigureAwait(false);
        }

        protected PageRequest ExportRequest => new PageRequest
        {
            Filter = string.Join(" or ", _selectedIdList.Select(id => $"Id eq '{id}'")),
            Expand = ExportExpand
        };

        protected override async Task OnInitializedAsync()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            await base.OnInitializedAsync().ConfigureAwait(false);
            _pageRequest = new PageRequest
            {
                Select = SelectProperties,
                Expand = Expand,
                Skip = 0,
                Take = 50
            };
            await GetEntityList(_pageRequest)
                .ConfigureAwait(false);

            GridState.OnHeaderClicked += GridState_OnHeaderClicked;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                const int margin = 95;
                return JSRuntime.InvokeVoidAsync("browserInteropt.onScrollEnd", DotNetObjectReference.Create(this), margin)
                    .AsTask();
            }
            return Task.CompletedTask;
        }


        protected Task OnFilterChanged(string filter)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
                       
            return Task.Delay(500, token)
                .ContinueWith(async task =>
                {
                    _pageRequest.Filter = CreateRequestFilter(filter);
                    _allItemsLoaded = false;
                    _pageRequest.Skip = 0;
                    EntityList = [];

                    await GetEntityList(_pageRequest, token).ConfigureAwait(false);
                }, TaskScheduler.Default);
        }

        protected virtual string CreateRequestFilter(string filter)
        {
            var propertyArray = SelectProperties.Split(',');
            var expressionArray = new string[propertyArray.Length];
            for (int i = 0; i < propertyArray.Length; i++)
            {
                expressionArray[i] = $"contains({propertyArray[i]},'{filter.Replace("'", "''")}')";
            }
            return string.Join(" or ", expressionArray);
        }

        protected void OnItemSelected(string id, bool isSelected)
        {
            if (isSelected)
            {
                _selectedIdList.Add(id);
                return;
            }
            var selectId = _selectedIdList.FirstOrDefault(i => i == id);
            if (selectId != null)
            {
                _selectedIdList.Remove(selectId);
            }
        }

        protected virtual void OnRowClicked(T entity)
        {
            if (entity is not IEntityId entityWithId)
            {
                throw new InvalidOperationException($"The identity type {typeof(T).Name} is not a 'IEntityId', override this method to navigate to the identity page.");
            }
            NavigationManager.NavigateTo($"{typeof(T).Name.ToLower()}/{entityWithId.Id}");
        }



        protected virtual string LocalizeEntityProperty<TEntityResource>(ILocalizable<TEntityResource> entity, string value, EntityResourceKind kind) where TEntityResource: IEntityResource
        {
            return entity.Resources.FirstOrDefault(r => r.ResourceKind == kind && r.CultureId == CultureInfo.CurrentCulture.Name)?.Value ?? value;
        }

        private async Task GetEntityList(PageRequest pageRequest, CancellationToken token = default)
        {
            var page = await AdminStore.GetAsync(pageRequest, token)
                            .ConfigureAwait(false);

            if (token.IsCancellationRequested )
            {
                return;
            }

            EntityList = EntityList.Concat(page.Items);

            await InvokeAsync(() => StateHasChanged())
                .ConfigureAwait(false);

            _pageRequest.Skip += _pageRequest.Take;
            _allItemsLoaded = EntityList.Count() == page.Count;

            if (!_allItemsLoaded && !await JSRuntime.InvokeAsync<bool>("browserInteropt.isScrollable", 0))
            {
                // load next page if document heigth < windows heigth
                await GetEntityList(_pageRequest).ConfigureAwait(false);
            }
        }

        private async Task GridState_OnHeaderClicked(SortEventArgs e)
        {
            EntityList = [];
            _allItemsLoaded = false;
            _pageRequest.Skip = 0;
            _pageRequest.OrderBy = e.OrderBy;
            await GetEntityList(_pageRequest)
                .ConfigureAwait(false);
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GridState.OnHeaderClicked -= GridState_OnHeaderClicked;
                    _cancellationTokenSource?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
