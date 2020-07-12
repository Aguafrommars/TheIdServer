using Aguacongas.IdentityServer.Store.Entity;
using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Import
    {
        private MarkupString _error;
        private ImportResult _result;
        async Task HandleFileSelected(IFileListEntry[] files)
        {
            using var content = new MultipartFormDataContent();
            foreach(var file in files)
            {
                content.Add(new StreamContent(file.Data), "files", file.Name);
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
