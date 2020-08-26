// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class SelectableItem<TItem>: IDisposable
    {
        private bool _value;
        private bool disposedValue;

        private bool Value
        {
            get => _value;
            set
            {
                _value = value;
                Selected.InvokeAsync(value);
            }
        }

        [Parameter]
        public TItem Item { get; set; }

        [Parameter]
        public RenderFragment<TItem> ChildContent { get; set; }

        [Parameter]
        public GridState GridState { get; set; }

        [Parameter]
        public EventCallback<bool> Selected { get; set; }

        protected override void OnInitialized()
        {
            Value = GridState.AllSelected;
            GridState.OnSelectAllClicked += GridState_OnSelectAllClicked;
            base.OnInitialized();
        }

        private Task GridState_OnSelectAllClicked(bool value)
        {
            Value = value;
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GridState.OnSelectAllClicked -= GridState_OnSelectAllClicked;
                    Selected.InvokeAsync(false);
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
