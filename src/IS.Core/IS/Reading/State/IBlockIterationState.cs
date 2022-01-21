using IS.Reading.Navigation;

namespace IS.Reading.State;

public interface IBlockIterationState
{
    Stack<object?> BackwardStack { get; }
    int? CurrentNodeIndex { get; set; }
    INode? CurrentNode { get; set; }
}
