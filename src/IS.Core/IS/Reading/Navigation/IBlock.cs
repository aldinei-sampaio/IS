namespace IS.Reading.Navigation;

public interface IBlock
{
    IReadOnlyList<INode> Nodes { get; }
    Stack<INode> BackwardStack { get; }
    int? CurrentNodeIndex { get; set; }
    INode? CurrentNode { get; set; }
}
