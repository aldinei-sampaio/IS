using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MoodNode : INode
{
    public MoodType? MoodType { get; }

    public MoodNode(MoodType? moodType)
        => (MoodType) = (moodType);

    private static async Task<object?> ApplyStateAsync(INavigationContext context, MoodType? moodType)
    {
        var oldState = context.State.MoodType;

        if (oldState == moodType)
            return null;

        if (moodType.HasValue)
        {
            if (oldState.HasValue || moodType.Value != Reading.MoodType.Normal)
            {
                var @event = new MoodChangeEvent(context.State.PersonName!, context.State.IsProtagonist(), moodType.Value);
                await context.Events.InvokeAsync<IMoodChangeEvent>(@event);
            }
        }

        context.State.MoodType = moodType;

        return moodType;
    }

    public Task<object?> EnterAsync(INavigationContext context)
        => ApplyStateAsync(context, MoodType);

    public Task EnterAsync(INavigationContext context, object? state)
        => ApplyStateAsync(context, state as MoodType?);
}
