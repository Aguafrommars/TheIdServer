// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class AuthorizeDropDownButton
    {
        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string CssSubClass { get; set; }

        [Parameter]
        public IEnumerable<string> Values { get; set; }

        private string GetActiveClass(string value)
        {
            return value == Value ? "active" : null;
        }

        private void SetSelectValue(string value)
        {
            CurrentValue = value;
        }
    }
}
