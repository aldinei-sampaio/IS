using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class TimedPauseNode : ITimedPauseNode
    {
        public TimedPauseNode(TimeSpan duration)
            => Duration = duration;

        public TimeSpan Duration { get; }

        public async Task<object?> EnterAsync(INavigationContext context)
        {
            await context.Events.InvokeAsync<ITimedPauseEvent>(new TimedPauseEvent(Duration));
            return null;
        }

        public Task EnterAsync(INavigationContext context, object? state)
            => Task.CompletedTask;
    }
}
