using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BackgroundDismissNode : INode
{
    public INode BackgroundChangeNode { get; }

    public BackgroundDismissNode()
        => BackgroundChangeNode = new BackgroundChangeNode(BackgroundState.Empty, null);

    public BackgroundDismissNode(INode node)
        => BackgroundChangeNode = node;

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var reversedNode = await BackgroundChangeNode.EnterAsync(context);
        if (ReferenceEquals(BackgroundChangeNode, reversedNode))
            return this;
        return new BackgroundDismissNode(reversedNode);
    }
}
