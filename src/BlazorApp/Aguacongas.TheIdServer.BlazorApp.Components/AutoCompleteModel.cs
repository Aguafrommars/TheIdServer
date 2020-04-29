using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public abstract class AutoCompleteModel<T> : ComponentBase, IDisposable
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public T Entity { get; set; }

        [Parameter]
        public EventCallback DeleteClicked { get; set; }

        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        [CascadingParameter]
        EditContext EditContext { get; set; }

        [JSInvokable]
        public Task EnterKeyPressed()
        {
            return SetSelectedValue(SelectedValue);
        }

        protected abstract bool IsReadOnly { get; }

        protected abstract string PropertyName { get; }

        protected string Id { get; } = Guid.NewGuid().ToString();

        protected string SelectedValue { get; set; }

        protected IEnumerable<string> FilteredValues { get; private set; }

        protected string FieldClass
            => EditContext.FieldCssClass(_fieldIdentifier);


        private CancellationTokenSource _cancellationTokenSource;


        private FieldIdentifier _fieldIdentifier;
        protected Task SetSelectedValue(string value)
        {
            SetValue(value);
            EditContext.NotifyFieldChanged(_fieldIdentifier);
            FilteredValues = null;
            return ValueChanged.InvokeAsync(Entity);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _fieldIdentifier = new FieldIdentifier(Entity, PropertyName);
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

            return Task.Delay(250, token)
                    .ContinueWith(async task =>
                    {
                        if (task.IsCanceled)
                        {
                            return;
                        }
                        FilteredValues = await GetFilteredValues(SelectedValue)
                            .ConfigureAwait(false);
                        if (FilteredValues.Any())
                        {
                            await JSRuntime.InvokeVoidAsync("bootstrapInteropt.showDropDownMenu", Id);
                        }
                       await InvokeAsync(StateHasChanged);
                    }, TaskScheduler.Default);
        }

        protected Task OnInputChanged(ChangeEventArgs e)
        {
            SelectedValue = e.Value as string;
            SetValue(SelectedValue);
            EditContext.NotifyFieldChanged(_fieldIdentifier);
            return Filter();
        }

        protected abstract void SetValue(string inputValue);

        protected abstract Task<IEnumerable<string>> GetFilteredValues(string term);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
