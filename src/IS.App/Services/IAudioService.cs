using Microsoft.JSInterop;

namespace IS.App.Services;

public interface IAudioService
{
    bool IsEnabled { get; }
    Task InitializeAsync();
    Task SetEnabledAsync(bool enabled, string? musicUrl);
    Task PlayAsync(string musicUrl);
    Task StopAsync();
}

public class AudioService(IJSRuntime jsRuntime) : IAudioService, IAsyncDisposable
{
    private IJSObjectReference? module;
    private bool initialized;

    public bool IsEnabled { get; private set; } = true;

    public async Task InitializeAsync()
    {
        if (initialized)
            return;
        initialized = true;
        var m = await GetModuleAsync();
        IsEnabled = await m.InvokeAsync<bool>("loadPreference");
    }

    public async Task SetEnabledAsync(bool enabled, string? musicUrl)
    {
        IsEnabled = enabled;
        var m = await GetModuleAsync();
        await m.InvokeVoidAsync("savePreference", enabled);
        if (enabled)
        {
            if (musicUrl != null)
                await m.InvokeVoidAsync("play", musicUrl);
        }
        else
        {
            await m.InvokeVoidAsync("saveAndPause");
        }
    }

    public async Task PlayAsync(string musicUrl)
    {
        var m = await GetModuleAsync();
        await m.InvokeVoidAsync("play", musicUrl);
    }

    public async Task StopAsync()
    {
        var m = await GetModuleAsync();
        await m.InvokeVoidAsync("stop");
    }

    private async Task<IJSObjectReference> GetModuleAsync()
    {
        module ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/audioService.js");
        return module;
    }

    public async ValueTask DisposeAsync()
    {
        if (module != null)
            await module.DisposeAsync();
    }
}
