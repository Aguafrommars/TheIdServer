using Microsoft.JSInterop;
using System.Threading.Tasks;
using System;

namespace Aguacongas.TheIdServer.BlazorApp.Services;
public class ThemeService {
    private readonly IJSRuntime _runtime;

    public string Theme { get; private set; }
    public event EventHandler ThemeChanged;

    public ThemeService(IJSRuntime runtime) {
        _runtime = runtime;
    }

    public async Task SetThemeAsync(string theme) {
        Theme = theme;
        await _runtime.InvokeVoidAsync("setTheme", theme).ConfigureAwait(false);
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task InitAsync() {
        Theme = await _runtime.InvokeAsync<string>("getTheme").ConfigureAwait(false);
        await SetThemeAsync(Theme).ConfigureAwait(false);
    }
}