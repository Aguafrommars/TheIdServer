// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public abstract class AutoCompleteModel<T> : InputText
    {
        private FieldIdentifier _fieldIdentifier;

        [Inject]
        protected IStringLocalizerAsync<AutoCompleteModel<T>> Localizer { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public T Entity { get; set; }

        [Parameter]
        public EventCallback DeleteClicked { get; set; }

        [Parameter]
        public EventCallback<T> EntityChanged { get; set; }

        [JSInvokable]
        public Task EnterKeyPressed()
        {
            return SetSelectedValue(CurrentValue);
        }

        protected abstract bool IsReadOnly { get; }

        protected abstract string PropertyName { get; }

        protected string Id { get; } = Guid.NewGuid().ToString();

        protected IEnumerable<string> FilteredValues { get; private set; }

        protected string FieldClass
            => EditContext.FieldCssClass(_fieldIdentifier);


        protected async Task SetSelectedValue(string value)
        {
            SetValue(value);
            FilteredValues = null;
            await EntityChanged.InvokeAsync(Entity).ConfigureAwait(false);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _fieldIdentifier = base.FieldIdentifier;
        }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !IsReadOnly)
            {
                return JSRuntime.InvokeVoidAsync("browserInteropt.preventEnterKeyPress", Id, DotNetObjectReference.Create(this))
                    .AsTask();
            }
            return Task.CompletedTask;
        }

        protected Task Filter()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            return Task.Delay(200, token)
                    .ContinueWith(async task =>
                    {
                        if (task.IsCanceled)
                        {
                            return;
                        }
                        FilteredValues = await GetFilteredValues(CurrentValueAsString)
                            .ConfigureAwait(false);
                       await JSRuntime.InvokeVoidAsync("bootstrapInteropt.showDropDownMenu", Id);
                       await InvokeAsync(StateHasChanged).ConfigureAwait(false);
                    }, TaskScheduler.Default);
        }

        protected virtual Task OnInputChanged(ChangeEventArgs e)
        {
            CurrentValue = e.Value as string;
            SetValue(CurrentValue);
            EditContext.NotifyFieldChanged(FieldIdentifier);
            return Filter();
        }

        protected abstract void SetValue(string inputValue);

        protected abstract Task<IEnumerable<string>> GetFilteredValues(string term);

        private CancellationTokenSource _cancellationTokenSource;


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cancellationTokenSource?.Dispose();
                }

                disposedValue = true;
            }
        }
        #endregion
    }
}
