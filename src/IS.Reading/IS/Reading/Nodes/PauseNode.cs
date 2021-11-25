using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class PauseNode : IPauseNode
    {
        public PauseNode(TimeSpan? duration, ICondition? when)
            => (Duration, When) = (duration, when);

        public ICondition? When { get; }

        public TimeSpan? Duration { get; }

        public Task<INode> EnterAsync(INavigationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
