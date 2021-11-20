using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class BackgroundScrollNode : INode
    {
        public BackgroundScrollNode(ICondition? condition)
            => When = condition;

        public ICondition? When { get; }

        public ICondition? While => null;

        public IBlock? ChildBlock => null;

        public Task<INode> EnterAsync(IContext context)
        {
            throw new NotImplementedException();
        }

        public Task LeaveAsync(IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
