using IS.Reading.Navigation;

namespace IS.Reading.State;

public interface IBlockState
{
    Stack<INode> BackwardStack { get; }
    int? CurrentNodeIndex { get; set; }
    INode? CurrentNode { get; set; }
}
