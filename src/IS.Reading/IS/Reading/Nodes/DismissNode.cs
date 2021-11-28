using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class DismissNode<T> : INode where T : INode
{
    public static DismissNode<T> Create(T node)
        => new DismissNode<T>(node);

    public INode ChangeNode { get; }

    public DismissNode(INode changeNode)
        => ChangeNode = changeNode;

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var reversedNode = await ChangeNode.EnterAsync(context);
        if (ReferenceEquals(ChangeNode, reversedNode))
            return this;
        return new DismissNode<T>(reversedNode);
    }
}