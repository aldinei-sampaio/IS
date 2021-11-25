using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class TimedPauseNode : ITimedPauseNode
    {
        public TimedPauseNode(TimeSpan duration, ICondition? when)
            => (Duration, When) = (duration, when);

        public ICondition? When { get; }

        public TimeSpan Duration { get; }

        public async Task<INode> EnterAsync(INavigationContext context)
        {
            await context.Events.InvokeAsync<ITimedPauseEvent>(new TimedPauseEvent(Duration));
            return this;
        }
    }
}
