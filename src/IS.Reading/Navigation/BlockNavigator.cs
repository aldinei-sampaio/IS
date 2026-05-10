using IS.Reading.State;

namespace IS.Reading.Navigation;

public class BlockNavigator : IBlockNavigator
{
    private class BlockedNode
    {
        public static BlockedNode Instance { get; } = new BlockedNode();
        private BlockedNode()
        {
        }
    }

    private readonly record struct IndexedEntry(int NodeIndex, object? State);

    public async Task<INode?> MoveAsync(IBlock block, IBlockState blockState, INavigationContext context, bool forward)
    {
        context.State.CurrentBlockId = block.Id;

        if (forward)
        {
            while (true)
            {
                var node = await MoveNextAsync(block, context, blockState.GetCurrentIteration());
                if (node is not null || block.While is null || !block.While.Evaluate(context.Variables))
                    return node;
                blockState.MoveToNextIteration();
            }
        }
        else
        {
            while (true)
            {
                var node = await MovePreviousAsync(block, context, blockState.GetCurrentIteration());

                if (node is not null || block.While is null || !blockState.MoveToPreviousIteration())
                    return node;
            }
        }
    }

    private static async Task<INode?> MoveNextAsync(IBlock block, INavigationContext context, IBlockIterationState blockState)
    {
        await LeaveCurrentNodeAsync(blockState, context);

        var item = GetNextValidNode(block, context, blockState);

        if (item is null)
            return null;

        var reverseState = await item.EnterAsync(context);
        blockState.BackwardStack.Push(new IndexedEntry(blockState.CurrentNodeIndex!.Value, reverseState));

        return item;
    }

    private static async Task LeaveCurrentNodeAsync(IBlockIterationState blockState, INavigationContext context)
    {
        if (blockState.CurrentNode is null)
            return;

        await blockState.CurrentNode.LeaveAsync(context);
    }

    private static INode? GetNextNode(IBlock block, IBlockIterationState blockState)
    {
        var nextIndex = blockState.CurrentNodeIndex.HasValue ? blockState.CurrentNodeIndex.Value + 1 : 0;
        if (nextIndex >= block.Nodes.Count)
        {
            blockState.CurrentNodeIndex = null;
            blockState.CurrentNode = null;
            return null;
        }

        var node = block.Nodes[nextIndex];

        blockState.CurrentNodeIndex = nextIndex;
        blockState.CurrentNode = node;
        return node;
    }

    private static INode? GetNextValidNode(IBlock block, INavigationContext context, IBlockIterationState blockState)
    {
        while (true)
        {
            var item = GetNextNode(block, blockState);
            if (item == null)
                return null;

            if (item.ChildBlock is null || item.ChildBlock.While is null || item.ChildBlock.While.Evaluate(context.Variables))
                return item;

            blockState.BackwardStack.Push(BlockedNode.Instance);
        }
    }

    private static async Task<INode?> MovePreviousAsync(IBlock block, INavigationContext context, IBlockIterationState blockState)
    {
        await LeaveCurrentNodeAsync(blockState, context);

        while (true)
        {
            if (!blockState.BackwardStack.TryPop(out var item))
            {
                blockState.CurrentNodeIndex = null;
                blockState.CurrentNode = null;
                return null;
            }

            if (item is not IndexedEntry entry)
                continue;

            var node = block.Nodes[entry.NodeIndex];

            if (node is IPauseNode)
            {
                if (blockState.SkipFirstPause)
                {
                    blockState.SkipFirstPause = false;
                    continue;
                }
                // Re-push para preservar o marcador de parada para futuros passos de retorno.
                blockState.BackwardStack.Push(entry);
                blockState.CurrentNodeIndex = entry.NodeIndex;
                blockState.CurrentNode = node;
                await node.EnterAsync(context, entry.State);
                return node;
            }

            blockState.SkipFirstPause = false;
            blockState.CurrentNodeIndex = entry.NodeIndex > 0 ? entry.NodeIndex - 1 : null;
            blockState.CurrentNode = node;

            await node.EnterAsync(context, entry.State);

            return node;
        }
    }
}
