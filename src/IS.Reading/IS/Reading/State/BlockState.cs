using IS.Reading.Navigation;

namespace IS.Reading.State;

public class BlockState : IBlockState
{
    public Stack<INode> BackwardStack { get; } = new();

    public int? CurrentNodeIndex { get; set; }

    public INode? CurrentNode { get; set; }
}
