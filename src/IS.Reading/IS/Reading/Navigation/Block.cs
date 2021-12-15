namespace IS.Reading.Navigation;

public class Block : IBlock
{
    public Queue<INode> ForwardQueue { get; }

    public Stack<INode> ForwardStack { get; } = new();

    public Stack<INode> BackwardStack { get; } = new();

    public INode? Current { get; set; } = null;

    public Block(IEnumerable<INode> nodes)
    {
        ForwardQueue = new(nodes);
    }

    public Block(INode node1, INode node2)
    {
        ForwardQueue = new();
        ForwardQueue.Enqueue(node1);
        ForwardQueue.Enqueue(node2);
    }
}
