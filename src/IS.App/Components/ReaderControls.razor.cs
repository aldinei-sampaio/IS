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

    [Parameter]
    public int SlideCount { get; set; }

    [Parameter]
    public EventCallback GoBackRequested { get; set; }

    [Parameter]
    public EventCallback AdvanceRequested { get; set; }

    [Inject]
    private IAudioService AudioService { get; set; } = default!;

    [Inject]
    private IAssetManager AssetManager { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private bool isVisible;
    private bool isIndicatorVisible;
    private string? lastMusicName;
    private CancellationTokenSource? timerCts;
    private Func<IMusicChangeEvent, Task>? musicHandler;

    protected override async Task OnInitializedAsync()
    {
        await AudioService.InitializeAsync();
        musicHandler = HandleMusicChange;
        Storyboard.Events.Subscribe(musicHandler);
    }

    // Chamado pelo BookReader após cada avanço ou recuo de slide
    public void OnSlideChanged(bool forward)
    {
        if (forward)
            isVisible = false;
        isIndicatorVisible = false;
        StartInactivityTimer();
    }

    private void OnUpperAreaTap()
    {
        timerCts?.Cancel();
        timerCts = null;
        isVisible = true;
        // isIndicatorVisible permanece false: toque no header exibe apenas os botões de nav
        InvokeAsync(StateHasChanged);
    }

    private void StartInactivityTimer()
    {
        timerCts?.Cancel();
        timerCts = new CancellationTokenSource();
        _ = InactivityTimerAsync(timerCts.Token);
    }

    private async Task InactivityTimerAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(5000, ct);
            isVisible = true;
            isIndicatorVisible = true;
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

    private async Task GoBack() => await GoBackRequested.InvokeAsync();

    private async Task OnIndicatorTap() => await AdvanceRequested.InvokeAsync();

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
