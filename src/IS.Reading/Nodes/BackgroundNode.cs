using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BackgroundNode(
    IBackgroundState state,
    BackgroundAnimation animation = BackgroundAnimation.None,
    string? flashColor = null
) : INode
{
    public IBackgroundState State { get; } = state;
    public BackgroundAnimation Animation { get; } = animation;
    public string? FlashColor { get; } = flashColor;

    private static async Task ApplyStateAsync(INavigationContext context, IBackgroundState state, BackgroundAnimation animation, string? flashColor)
    {
        await context.Events.InvokeAsync<IBackgroundChangeEvent>(new BackgroundChangeEvent(state, animation, flashColor));
        context.State.Background = state;
    }

    public async Task<object?> EnterAsync(INavigationContext context)
    {
        var oldState = context.State.Background;
        if (oldState == State)
            return oldState;

        await ApplyStateAsync(context, State, Animation, FlashColor);

        return oldState;
    }

    public async Task EnterAsync(INavigationContext context, object? state)
    {
        if (state is IBackgroundState backgroundState)
            await ApplyStateAsync(context, backgroundState, BackgroundAnimation.None, null);
    }
}
