using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class BlockNode(IBlock childBlock) : INode
{
    public IBlock? ChildBlock { get; } = childBlock;
}
