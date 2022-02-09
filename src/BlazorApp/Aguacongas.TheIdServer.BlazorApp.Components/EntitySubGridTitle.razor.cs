// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class EntitySubGridTitle<T>
    {
        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public ICollection<T> Collection { get; set; }

        [Parameter]
        public Func<T> CreateModel { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }

        protected virtual void OnAddItenClicked()
        {
            var model = CreateModel();
            Collection.Add(model);
            HandleModificationState.EntityCreated(model);
        }
    }
}
