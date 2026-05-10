using IS.App.Services;
using IS.Reading.Events;
using IS.Reading.Navigation;
using Microsoft.AspNetCore.Components;

namespace IS.App.Components;

public partial class ReaderControls : IDisposable
{
    [Parameter, EditorRequired]
    public IStoryboard Storyboard { get; set; } = default!;

    [Parameter, EditorRequired]
    public string BookId { get; set; } = string.Empty;

    [Inject]
    private IAudioService AudioService { get; set; } = default!;

    [Inject]
    private IAssetManager AssetManager { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private bool isVisible;
    private string? lastMusicName;
    private CancellationTokenSource? timerCts;
    private Func<IMusicChangeEvent, Task>? musicHandler;

    protected override async Task OnInitializedAsync()
    {
        await AudioService.InitializeAsync();
        musicHandler = HandleMusicChange;
        Storyboard.Events.Subscribe(musicHandler);
    }

    // Chamado pelo BookReader após cada avanço de slide
    public void OnSlideChanged()
    {
        isVisible = false;
        StartTimer(showImmediately: false);
    }

    private void OnUpperAreaTap() => StartTimer(showImmediately: true);

    private void StartTimer(bool showImmediately)
    {
        timerCts?.Cancel();
        timerCts = new CancellationTokenSource();
        if (showImmediately)
        {
            isVisible = true;
            InvokeAsync(StateHasChanged);
        }
        _ = RunTimerAsync(showImmediately, timerCts.Token);
    }

    private async Task RunTimerAsync(bool showImmediately, CancellationToken ct)
    {
        try
        {
            if (!showImmediately)
            {
                await Task.Delay(5000, ct);
                isVisible = true;
                await InvokeAsync(StateHasChanged);
            }
            await Task.Delay(5000, ct);
            isVisible = false;
            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException) { }
    }

    private async Task HandleMusicChange(IMusicChangeEvent e)
    {
        lastMusicName = e.MusicName;
        if (!AudioService.IsEnabled)
            return;
        if (e.MusicName != null)
            await AudioService.PlayAsync(AssetManager.GetMusicUrl(BookId, e.MusicName));
        else
            await AudioService.StopAsync();
    }

    private void GoHome() => Navigation.NavigateTo($"/book/{BookId}");

    private void GoTrophies() { }

    private async Task GoBack() => await Storyboard.MoveAsync(false);

    private async Task ToggleSound()
    {
        if (AudioService.IsEnabled)
        {
            await AudioService.SetEnabledAsync(false, null);
        }
        else
        {
            var musicUrl = lastMusicName != null ? AssetManager.GetMusicUrl(BookId, lastMusicName) : null;
            await AudioService.SetEnabledAsync(true, musicUrl);
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        timerCts?.Cancel();
        if (musicHandler == null)
            return;
        try
        {
            Storyboard.Events.Unsubscribe(musicHandler);
        }
        catch (EventHandlerNotFoundException) { }
        musicHandler = null;
    }
}
