﻿using IS.Reading.State;

namespace IS.Reading.Navigation;

public class BlockNavigator : IBlockNavigator
{
    private class BlockedNode : INode
    {
    }

    public async Task<INode?> MoveAsync(IBlock block, INavigationContext context, bool forward)
    {
        context.State.CurrentBlockId = block.Id;
        var blockState = context.State.BlockStates[block.Id, 0];
        if (forward)
            return await MoveNextAsync(block, context, blockState);
        else
            return await MovePreviousAsync(block, context, blockState);
    }

    private static async Task<INode?> MoveNextAsync(IBlock block, INavigationContext context, IBlockState blockState)
    {
        await LeaveCurrentNodeAsync(blockState, context);

        var item = GetNextValidNode(block, context, blockState);

        if (item is null)
            return null;

        var reverse = await item.EnterAsync(context);
        if (reverse is not null)
            blockState.BackwardStack.Push(reverse);

        return item;
    }

    private static async Task LeaveCurrentNodeAsync(IBlockState blockState, INavigationContext context)
    {
        if (blockState.CurrentNode is null)
            return;

        await blockState.CurrentNode.LeaveAsync(context);
    }

    private static INode? GetNextNode(IBlock block, IBlockState blockState)
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

    private static INode? GetNextValidNode(IBlock block, INavigationContext context, IBlockState blockState)
    {
        for (; ; )
        {
            var item = GetNextNode(block, blockState);
            if (item == null)
                return null;

            if (item.When == null || item.When.Evaluate(context.Variables))
                return item;

            blockState.BackwardStack.Push(new BlockedNode());
        }
    }

    private static async Task<INode?> MovePreviousAsync(IBlock block, INavigationContext context, IBlockState blockState)
    {
        await LeaveCurrentNodeAsync(blockState, context);

        var item = GetValidPreviousNode(block, blockState);

        if (item is null)
            return null;

        await item.EnterAsync(context);

        return item;
    }

    private static INode? GetValidPreviousNode(IBlock block, IBlockState blockState)
    {
        var index = blockState.CurrentNodeIndex ?? (block.Nodes.Count - 1);

        for (; ; )
        {
            if (!blockState.BackwardStack.TryPop(out var item))
            {
                blockState.CurrentNodeIndex = null;
                blockState.CurrentNode = null;
                return null;
            }

            index--;

            if (item is not BlockedNode)
            {
                blockState.CurrentNodeIndex = index >= 0 ? index : null;
                blockState.CurrentNode = item;
                return item;
            }
        }
    }
}
