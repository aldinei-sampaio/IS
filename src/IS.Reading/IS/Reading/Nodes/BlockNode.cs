using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class BlockNode : INode
{
    public BlockNode(IBlock childBlock)
        => ChildBlock = childBlock;

    public IBlock? ChildBlock { get; }
}
