using IS.Reading.Conditions;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class BlockNode : INode
    {
        public BlockNode(IBlock childBlock, ICondition? when)
            => (ChildBlock, When) = (childBlock, when);

        public ICondition? When { get; }

        public IBlock? ChildBlock { get; }
    }
}
