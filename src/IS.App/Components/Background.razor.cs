using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;
using Microsoft.AspNetCore.Components;

namespace IS.App.Components;

public partial class Background : IDisposable
{
    [Parameter, EditorRequired]
    public IStoryboard Storyboard { get; set; } = default!;

    [Parameter, EditorRequired]
    public string BookId { get; set; } = string.Empty;

    private BackgroundType type = BackgroundType.Undefined;
    private string name = string.Empty;
    private BackgroundPosition displayPosition = BackgroundPosition.Left;
    private BackgroundPosition targetPosition = BackgroundPosition.Left;
    private bool pendingScroll;
    private bool isScrolling;
    private CancellationTokenSource? scrollCts;

    private Func<IBackgroundChangeEvent, Task>? changeHandler;
    private Func<IBackgroundScrollEvent, Task>? scrollHandler;

    protected override void OnInitialized()
    {
        changeHandler = HandleChange;
        scrollHandler = HandleScroll;
        Storyboard.Events.Subscribe(changeHandler);
        Storyboard.Events.Subscribe(scrollHandler);
    }

    public void Dispose()
    {
        scrollCts?.Cancel();
        if (changeHandler == null)
            return;
        try
        {
            Storyboard.Events.Unsubscribe(changeHandler);
            Storyboard.Events.Unsubscribe(scrollHandler!);
        }
        catch (EventHandlerNotFoundException) { }
        changeHandler = null;
    }

    private Task HandleChange(IBackgroundChangeEvent e)
    {
        scrollCts?.Cancel();
        isScrolling = false;
        pendingScroll = false;
        type = e.State.Type;
        name = e.State.Name;
        displayPosition = e.State.Position != BackgroundPosition.Undefined
            ? e.State.Position
            : BackgroundPosition.Left;
        targetPosition = displayPosition;
        return Task.CompletedTask;
    }

    private Task HandleScroll(IBackgroundScrollEvent e)
    {
        if (e.Position == BackgroundPosition.Undefined || e.Position == displayPosition)
            return Task.CompletedTask;
        targetPosition = e.Position;
        pendingScroll = true;
        return Task.CompletedTask;
    }

    // The animation requires two renders:
    // 1. First render shows the image at its initial position (displayPosition from HandleChange).
    // 2. OnAfterRenderAsync detects pendingScroll, updates displayPosition to targetPosition,
    //    enables the CSS transition, and re-renders. The browser then animates the change.
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (!pendingScroll)
            return Task.CompletedTask;

        pendingScroll = false;
        scrollCts?.Cancel();
        scrollCts = new CancellationTokenSource();
        var ct = scrollCts.Token;

        displayPosition = targetPosition;
        isScrolling = true;
        StateHasChanged();

        _ = FinishScrollAsync(ct);
        return Task.CompletedTask;
    }

    private async Task FinishScrollAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(1000, ct);
            isScrolling = false;
            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException) { }
    }

    private string GetStyle()
    {
        return type switch
        {
            BackgroundType.Color when !string.IsNullOrEmpty(name)
                => $"background-color: {name};",
            BackgroundType.Image when !string.IsNullOrEmpty(name)
                => $"background-image: url('assets/books/{BookId}/background/{name}.jpg'); " +
                   $"background-position: {(displayPosition == BackgroundPosition.Right ? "right" : "left")} center;",
            _ => string.Empty
        };
    }
}
