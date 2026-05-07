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
    private bool isScrolling;
    private string animationClass = string.Empty;
    private string? flashColor;
    private bool isFlashVisible;
    private bool isFlashFading;
    private CancellationTokenSource? animationCts;

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
        animationCts?.Cancel();
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

    private async Task HandleChange(IBackgroundChangeEvent e)
    {
        animationCts?.Cancel();
        animationCts = new CancellationTokenSource();
        var ct = animationCts.Token;

        isScrolling = false;
        animationClass = string.Empty;
        isFlashVisible = false;
        isFlashFading = false;
        type = e.State.Type;
        name = e.State.Name;
        displayPosition = e.State.Position != BackgroundPosition.Undefined
            ? e.State.Position
            : BackgroundPosition.Left;

        try
        {
            await ApplyAnimationAsync(e.Animation, e.FlashColor, ct);
        }
        catch (OperationCanceledException) { }
    }

    private async Task ApplyAnimationAsync(BackgroundAnimation animation, string? color, CancellationToken ct)
    {
        if (animation == BackgroundAnimation.Flash)
        {
            flashColor = color ?? "white";
            isFlashVisible = true;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(16, ct);
            isFlashFading = true;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(500, ct);
            isFlashVisible = false;
            isFlashFading = false;
            flashColor = null;
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            animationClass = animation switch
            {
                BackgroundAnimation.FadeIn   => "bg-fadein",
                BackgroundAnimation.Zoom     => "bg-zoom",
                BackgroundAnimation.Dissolve => "bg-dissolve",
                _                            => string.Empty
            };
            await InvokeAsync(StateHasChanged);
            if (animationClass.Length > 0)
            {
                await Task.Delay(500, ct);
                animationClass = string.Empty;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async Task HandleScroll(IBackgroundScrollEvent e)
    {
        if (e.Position == BackgroundPosition.Undefined || e.Position == displayPosition)
            return;

        animationCts?.Cancel();
        animationCts = new CancellationTokenSource();
        var ct = animationCts.Token;

        try
        {
            await Task.Delay(16, ct);
            displayPosition = e.Position;
            isScrolling = true;
            await InvokeAsync(StateHasChanged);
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
