using System.Diagnostics.CodeAnalysis;

namespace IS.Reading.Navigation;

public class BlockNavigator : IBlockNavigator
{
    private class BlockedNode : INode
    {
        public INode OriginalNode { get; }

        public BlockedNode(INode originalNode)
        {
            OriginalNode = originalNode;
        }

        [ExcludeFromCodeCoverage]
        public ICondition? When => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public ICondition? While => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public IBlock? ChildBlock => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public Task<INode> EnterAsync(INavigationContext context) => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public Task LeaveAsync(INavigationContext context) => throw new NotImplementedException();
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
        block.Current = item;

        if (item is null)
            return null;

        var reverse = await item.EnterAsync(context);
        if (reverse is not null)
            block.BackwardStack.Push(reverse);

        return item;
    }

    private static async Task LeaveCurrentNodeAsync(IBlock block, INavigationContext context)
    {
        if (block.Current is null)
            return;

        await block.Current.LeaveAsync(context);
        block.Current = null;
    }

    private static INode? GetNextNode(IBlock block)
    {
        if (block.ForwardStack.TryPop(out var stackItem))
            return stackItem;

        if (block.ForwardQueue.TryDequeue(out var queueItem))
            return queueItem;

        return null;
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

            block.BackwardStack.Push(new BlockedNode(item));
        }
    }

    private static async Task<INode?> MovePreviousAsync(IBlock block, INavigationContext context)
    {
        await LeaveCurrentNodeAsync(block, context);

        var item = GetValidPreviousNode(block);
        block.Current = item;

        if (item is null)
            return null;

        var reversed = await item.EnterAsync(context);
        if (reversed is not null)
            block.ForwardStack.Push(reversed);

        return item;
    }

    private static INode? GetValidPreviousNode(IBlock block)
    {
        for (; ; )
        {
            if (!block.BackwardStack.TryPop(out var item))
                return null;

            if (item is not BlockedNode blocked)
                return item;

            block.ForwardStack.Push(blocked.OriginalNode);
        }
    }
}
