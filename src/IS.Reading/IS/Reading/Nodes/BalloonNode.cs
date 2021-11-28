using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class BalloonNode : INode
{
    public BalloonType BallonType { get; }

    public BalloonNode(BalloonType ballonType, IBlock childBlock)
        => (BallonType, ChildBlock) = (ballonType, childBlock);

    public IBlock? ChildBlock { get; }

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var @event = new BalloonOpenEvent(BallonType, context.State.IsProtagonist());
        await context.Events.InvokeAsync<IBalloonOpenEvent>(@event);
        return this;
    }

    public async Task<INode> LeaveAsync(INavigationContext context)
    {
        var @event = new BalloonCloseEvent(BallonType, context.State.IsProtagonist());
        await context.Events.InvokeAsync<IBalloonCloseEvent>(@event);
        return this;
    }
}
