using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MoodNode : INode
{
    public MoodType? MoodType { get; }

    public MoodNode(MoodType? moodType)
        => (MoodType) = (moodType);

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var oldState = context.State.MoodType;

        if (oldState == MoodType)
            return this;

        if (MoodType.HasValue)
        {
            var @event = new MoodChangeEvent(context.State.PersonName!, context.State.IsProtagonist(), MoodType.Value);
            await context.Events.InvokeAsync<IMoodChangeEvent>(@event);
        }
            
        context.State.MoodType = MoodType;

        return new MoodNode(oldState);
    }
}
