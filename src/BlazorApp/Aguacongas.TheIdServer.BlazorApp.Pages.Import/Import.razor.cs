// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Import
{
    public partial class Import
    {
        private MarkupString _error;
        private ImportResult _result;
        async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            using var content = new MultipartFormDataContent();
            foreach(var file in e.GetMultipleFiles(e.FileCount))
            {
                content.Add(new StreamContent(file.OpenReadStream()), "files", file.Name);
            }

            var httpClient = _httpClientFactory.CreateClient("oidc");
            using var response = await httpClient.PostAsync($"{httpClient.BaseAddress}/import", content).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _error = new MarkupString($"{response.StatusCode}<br/>{responseContent.Replace("\n","<br/>")}");
            }
            else
            {
                _result = JsonSerializer.Deserialize<ImportResult>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }

            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }
    }
}
