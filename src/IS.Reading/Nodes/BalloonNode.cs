using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class BalloonNode(BalloonType balloonType, IBlock childBlock) : INode
{
    public BalloonType BallonType { get; } = balloonType;

    public IBlock? ChildBlock { get; } = childBlock;

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
    }
}
