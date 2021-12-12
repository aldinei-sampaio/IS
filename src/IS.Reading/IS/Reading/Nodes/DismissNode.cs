using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public static class DismissNode
{
    public static DismissNode<T> Create<T>(T node) where T : INode
        => new DismissNode<T>(node);
}

public class DismissNode<T> : INode where T : INode
{
    public T ChangeNode { get; }

    public DismissNode(T changeNode)
        => ChangeNode = changeNode;

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var reversedNode = (T)await ChangeNode.EnterAsync(context);
        if (ReferenceEquals(ChangeNode, reversedNode))
            return this;
        return new DismissNode<T>(reversedNode);
    }
}