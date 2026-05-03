using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MoodNode : INode
{
    public MoodType? MoodType { get; }

    public MoodNode(MoodType? moodType)
        => MoodType = moodType;

    private static async Task<object?> ApplyStateAsync(INavigationContext context, MoodType? moodType)
    {
        var oldState = context.State.MoodType;

        if (oldState == moodType)
            return oldState;

        if (moodType.HasValue)
        {
            if (oldState.HasValue || moodType.Value != Reading.MoodType.Normal)
            {
                var @event = new MoodChangeEvent(context.State.PersonName!, context.State.IsMainCharacter(), moodType.Value);
                await context.Events.InvokeAsync<IMoodChangeEvent>(@event);
            }
        }

        context.State.MoodType = moodType;

        return oldState;
    }

    public Task<object?> EnterAsync(INavigationContext context)
        => ApplyStateAsync(context, MoodType);

    public Task EnterAsync(INavigationContext context, object? state)
        => ApplyStateAsync(context, state as MoodType?);
}
