// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class ExportButton
    {
        [Parameter]
        public PageRequest Request { get; set; }

        [Parameter]
        public string EntityPath { get; set; }

        [Parameter]
        public string CssClass { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        private async Task Download()
        {
            var token = await _service.GetOneTimeToken().ConfigureAwait(false);
            var builder = new StringBuilder(_settings.ApiBaseUrl);
            if (!_settings.ApiBaseUrl.EndsWith('/'))
            {
                builder.Append('/');
            }
            builder.Append(EntityPath);
            var dictionary = typeof(PageRequest)
                .GetProperties()
                .Where(p => p.Name != nameof(PageRequest.Select) &&
                    p.Name != nameof(PageRequest.Take) &&
                    p.GetValue(Request) != null)
                .ToDictionary(p => p.Name.ToLowerInvariant(), p => p.GetValue(Request).ToString());
            dictionary.Add("format", "export");
            dictionary.Add("otk", token);
            var url = QueryHelpers.AddQueryString(builder.ToString(), dictionary);
            await _jsRuntime.InvokeVoidAsync("open", url, "_blank").ConfigureAwait(false);
        }
    }
}
