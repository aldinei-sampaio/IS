using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class BalloonTextNode : INode
{
    public string Text { get; }
    public BalloonType BalloonType { get; }

    public BalloonTextNode(string text, BalloonType ballonType)
        => (Text, BalloonType) = (text, ballonType);

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var @event = new BalloonTextEvent(Text, BalloonType, context.State.IsProtagonist());
        await context.Events.InvokeAsync<IBalloonTextEvent>(@event);
        return this;
    }
}
