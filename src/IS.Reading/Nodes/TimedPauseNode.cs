using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class TimedPauseNode(TimeSpan duration) : ITimedPauseNode
{
    public TimeSpan Duration { get; } = duration;

    public async Task<object?> EnterAsync(INavigationContext context)
    {
        await context.Events.InvokeAsync<ITimedPauseEvent>(new TimedPauseEvent(Duration));
        return null;
    }

    public Task EnterAsync(INavigationContext context, object? state)
        => Task.CompletedTask;
}
