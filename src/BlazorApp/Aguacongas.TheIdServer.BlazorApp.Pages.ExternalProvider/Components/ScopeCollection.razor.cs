// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.Components
{
    public partial class ScopeCollection
    {
        private string _value;

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public ICollection<string> Collection { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        [CascadingParameter]
        public Models.ExternalProvider Model { get; set; }

        private void OnDelete(string item)
        {
            Collection.Remove(item);
            HandleModificationState.EntityUpdated(Model);
        }

        private void OnInput(ChangeEventArgs e)
        {
            _value = e.Value as string;
        }

        private void OnInputChanged(ChangeEventArgs e)
        {
            OnInput(e);
            AddValue();
        }

        private void AddValue()
        {
            if (_value == null)
            {
                return;
            }
            Collection.Add(_value);
            HandleModificationState.EntityUpdated(Model);
            _value = null;
        }
    }
}
