using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public abstract class AutoCompleteModel<T> : ComponentBase, IDisposable
    {
        private Expression<Func<object>> _fieldExpression;
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
        public EventCallback<T> ValueChanged { get; set; }

        [Parameter]
        public Expression<Func<object>> FieldExpression 
        {
            get => _fieldExpression;
            set
            {
                _fieldExpression = value;
                _fieldIdentifier = FieldIdentifier.Create(value);
            }
        }

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


        protected async Task SetSelectedValue(string value)
        {
            SetValue(value);
            FilteredValues = null;
            await ValueChanged.InvokeAsync(Entity).ConfigureAwait(false);
            EditContext.NotifyFieldChanged(_fieldIdentifier);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (_fieldExpression == null)
            {
                _fieldIdentifier = new FieldIdentifier(Entity, PropertyName);
            }
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
                        FilteredValues = await GetFilteredValues(SelectedValue)
                            .ConfigureAwait(false);
                        if (FilteredValues.Any())
                        {
                            await JSRuntime.InvokeVoidAsync("bootstrapInteropt.showDropDownMenu", Id);
                        }
                       await InvokeAsync(StateHasChanged).ConfigureAwait(false);
                    }, TaskScheduler.Default);
        }

        protected virtual Task OnInputChanged(ChangeEventArgs e)
        {
            SelectedValue = e.Value as string;
            SetValue(SelectedValue);
            EditContext.NotifyFieldChanged(_fieldIdentifier);
            return Filter();
        }

        protected abstract void SetValue(string inputValue);

        protected abstract Task<IEnumerable<string>> GetFilteredValues(string term);

        private CancellationTokenSource _cancellationTokenSource;


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
