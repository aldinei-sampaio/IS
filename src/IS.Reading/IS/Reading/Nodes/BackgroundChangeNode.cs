using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BackgroundChangeNode : INode
{
    public IBackgroundState State { get; }

    public BackgroundChangeNode(IBackgroundState state, ICondition? when)
        => (State, When) = (state, when);

    public ICondition? When { get; }

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var oldState = context.State.Background;
        if (oldState == State)
            return this;

        await context.Events.InvokeAsync<IBackgroundChangeEvent>(new BackgroundChangeEvent(State));
        context.State.Background = State;

        return new BackgroundChangeNode(oldState, When);
    }
}
