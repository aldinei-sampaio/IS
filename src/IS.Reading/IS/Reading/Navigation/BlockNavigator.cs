namespace IS.Reading.Navigation;

public class BlockNavigator : IBlockNavigator
{
    private class BlockedNode : INode
    {
    }

    public async Task<INode?> MoveAsync(IBlock block, INavigationContext context, bool forward)
    {
        if (forward)
            return await MoveNextAsync(block, context);
        else
            return await MovePreviousAsync(block, context);
    }

    private static async Task<INode?> MoveNextAsync(IBlock block, INavigationContext context)
    {
        await LeaveCurrentNodeAsync(block, context);

        var item = GetNextValidNode(block, context);

        if (item is null)
            return null;

        var reverse = await item.EnterAsync(context);
        if (reverse is not null)
            block.BackwardStack.Push(reverse);

        return item;
    }

    private static async Task LeaveCurrentNodeAsync(IBlock block, INavigationContext context)
    {
        if (block.CurrentNode is null)
            return;

        await block.CurrentNode.LeaveAsync(context);
    }

    private static INode? GetNextNode(IBlock block)
    {
        var nextIndex = block.CurrentNodeIndex.HasValue ? block.CurrentNodeIndex.Value + 1 : 0;
        if (nextIndex >= block.Nodes.Count)
        {
            block.CurrentNodeIndex = null;
            block.CurrentNode = null;
            return null;
        }

        var node = block.Nodes[nextIndex];

        block.CurrentNodeIndex = nextIndex;
        block.CurrentNode = node;
        return node;
    }

    private static INode? GetNextValidNode(IBlock block, INavigationContext context)
    {
        for (; ; )
        {
            var item = GetNextNode(block);
            if (item == null)
                return null;

            if (item.When == null || item.When.Evaluate(context.Variables))
                return item;

            block.BackwardStack.Push(new BlockedNode());
        }
    }

    private static async Task<INode?> MovePreviousAsync(IBlock block, INavigationContext context)
    {
        await LeaveCurrentNodeAsync(block, context);

        var item = GetValidPreviousNode(block);

        if (item is null)
            return null;

        await item.EnterAsync(context);

        return item;
    }

    private static INode? GetValidPreviousNode(IBlock block)
    {
        var index = block.CurrentNodeIndex ?? (block.Nodes.Count - 1);

        for (; ; )
        {
            if (!block.BackwardStack.TryPop(out var item))
            {
                block.CurrentNodeIndex = null;
                block.CurrentNode = null;
                return null;
            }

            index--;

            if (item is not BlockedNode)
            {
                block.CurrentNodeIndex = index >= 0 ? index : null;
                block.CurrentNode = item;
                return item;
            }
        }
    }
}
