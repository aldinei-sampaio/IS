using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class BalloonNode : INode
{
    public BalloonType BallonType { get; }

    public BalloonNode(BalloonType ballonType, IBlock childBlock)
        => (BallonType, ChildBlock) = (ballonType, childBlock);

    public IBlock? ChildBlock { get; }

    public async Task<object?> EnterAsync(INavigationContext context)
    {
        var @event = new BalloonOpenEvent(BallonType, context.State.IsMainCharacter());
        await context.Events.InvokeAsync<IBalloonOpenEvent>(@event);
        return null;
    }

    public async Task LeaveAsync(INavigationContext context)
    {
        var @event = new BalloonCloseEvent(BallonType, context.State.IsMainCharacter());
        await context.Events.InvokeAsync<IBalloonCloseEvent>(@event);
        return;
    }
}
