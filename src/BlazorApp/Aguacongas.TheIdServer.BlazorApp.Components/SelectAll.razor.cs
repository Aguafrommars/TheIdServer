// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class SelectAll
    {
        private bool Value 
        {
            get => GridState.AllSelected;
            set
            {
                if (value == GridState.AllSelected)
                {
                    return;
                }
                GridState.AllSelected = value;

                GridState.SelectAllClicked(value);
            }
        }

        [Parameter]
        public GridState GridState { get; set; }
    }
}
