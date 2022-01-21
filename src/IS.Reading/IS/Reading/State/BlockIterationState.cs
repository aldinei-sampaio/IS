using IS.Reading.Navigation;

namespace IS.Reading.State;

public class BlockIterationState : IBlockIterationState
{
    public Stack<object?> BackwardStack { get; } = new();

    public int? CurrentNodeIndex { get; set; }

    public INode? CurrentNode { get; set; }
}
