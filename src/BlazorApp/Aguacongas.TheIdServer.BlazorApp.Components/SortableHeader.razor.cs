// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class SortableHeader : IDisposable
    {
        private string _class = null;
        private bool disposedValue;

        [Parameter]
        public string Property { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public GridState GridState { get; set; }

        [Parameter]
        public EventCallback<string> TermChanged { get; set; }

        void OnHeaderClick()
        {
            GridState.HeaderClicked(Property);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            GridState.OnHeaderClicked += GridState_OnHeaderClicked;
        }

        private Task GridState_OnHeaderClicked(Models.SortEventArgs e)
        {
            _class = GridState.GetHeaderArrowClassSuffix(Property);
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GridState.OnHeaderClicked -= GridState_OnHeaderClicked;
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
