using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MoodNode : INode
{
    public MoodType? MoodType { get; }

    public MoodNode(MoodType? moodType)
        => (MoodType) = (moodType);

    private async static Task RaiseEventAsync(INavigationContext context, MoodType mood)
    {
        var @event = new MoodChangeEvent(context.State.PersonName!, context.State.IsProtagonist(), mood);
        await context.Events.InvokeAsync<IMoodChangeEvent>(@event);
    }

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var oldState = context.State.MoodType;

        if (oldState == MoodType)
            return this;

        if (MoodType.HasValue)
        {
            if (oldState.HasValue || MoodType.Value != Reading.MoodType.Normal)
                await RaiseEventAsync(context, MoodType.Value);
        }

        context.State.MoodType = MoodType;

        return new MoodNode(oldState);
    }
}
