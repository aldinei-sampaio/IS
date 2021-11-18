using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public struct BlockNode : INode
    {
        public BlockNode(IBlock childBlock, ICondition? condition)
            => (ChildBlock, Condition) = (childBlock, condition);

        public ICondition? Condition { get; }

        public IBlock? ChildBlock { get; }

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
