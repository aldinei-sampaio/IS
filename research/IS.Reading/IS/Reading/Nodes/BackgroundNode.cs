using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BackgroundNode : INode
{
    public IBackgroundState State { get; }

    public BackgroundNode(IBackgroundState state)
        => State = state;

    private static async Task ApplyStateAsync(INavigationContext context, IBackgroundState state)
    {
        await context.Events.InvokeAsync<IBackgroundChangeEvent>(new BackgroundChangeEvent(state));
        context.State.Background = state;
    }

    public async Task<object?> EnterAsync(INavigationContext context)
    {
        var oldState = context.State.Background;
        if (oldState == State)
            return oldState;

        await ApplyStateAsync(context, State);

        return oldState;
    }

    public async Task EnterAsync(INavigationContext context, object? state)
    {
        if (state is IBackgroundState backgroundState)
            await ApplyStateAsync(context, backgroundState);
    }
}
