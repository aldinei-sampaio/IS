using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public struct PauseNode : IPauseNode
    {
        public PauseNode(ICondition? condition)
            => Condition = condition;

        public ICondition? Condition { get; }

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
