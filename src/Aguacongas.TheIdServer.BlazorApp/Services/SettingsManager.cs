using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class SettingsManager : IManageSettings
    {
        private readonly HttpClient _httpClient;

        public Settings Settings { get; private set; }
        public SettingsManager(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task InitializeAsync()
        {
            Settings = await _httpClient.GetJsonAsync<Settings>("settings.json")
                .ConfigureAwait(false);
        }
    }
}
