using IS.Reading.Navigation;

namespace IS.Reading.State;

public interface IBlockState
{
    Stack<string> BackwardStack { get; }
    int? CurrentNodeIndex { get; set; }
    INode? CurrentNode { get; set; }
}
