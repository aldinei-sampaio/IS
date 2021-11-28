using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MoodNode : INode
{
    public MoodType MoodType { get; }

    public MoodNode(MoodType moodType, IBlock childBlock)
        => (MoodType, ChildBlock) = (moodType, childBlock);

    public IBlock? ChildBlock { get; }

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        if (context.State.MoodType == MoodType)
            return this;

        var @event = new MoodChangeEvent(context.State.Person!, MoodType);
        await context.Events.InvokeAsync<IMoodChangeEvent>(@event);
        context.State.MoodType = MoodType;

        return this;
    }
}
