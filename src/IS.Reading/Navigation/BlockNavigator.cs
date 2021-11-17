namespace IS.Reading.Navigation
{
    public class BlockNavigator : IBlockNavigator
    {
        private class BlockedNode : INavigationNode
        {
            public INavigationNode OriginalNode { get; }

            public BlockedNode(INavigationNode originalNode)
            {
                OriginalNode = originalNode;
            }

            public ICondition? Condition => throw new NotImplementedException();

            public INavigationBlock? ChildBlock => throw new NotImplementedException();

            public Task<INavigationNode> EnterAsync(INavigationContext context)
                => throw new NotImplementedException();

            public Task LeaveAsync(INavigationContext context)
                => throw new NotImplementedException();
        }

        private static async Task LeaveCurrentNodeAsync(INavigationBlock block, INavigationContext context)
        {
            if (block.Current is null)
                return;

            await block.Current.LeaveAsync(context);
            block.Current = null;
        }

        public async Task<INavigationNode?> MoveNextAsync(INavigationBlock block, INavigationContext context)
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

        private static INavigationNode? GetNextNode(INavigationBlock block)
        {
            if (block.ForwardStack.TryPop(out var stackItem))
                return stackItem;

            if (block.ForwardQueue.TryDequeue(out var queueItem))
                return queueItem;

            return null;
        }

        private static INavigationNode? GetNextValidNode(INavigationBlock block, INavigationContext context)
        {
            for (; ; )
            {
                var item = GetNextNode(block);
                if (item == null)
                    return null;

                if (item.Condition == null || item.Condition.Evaluate(context.Variables))
                    return item;

                block.BackwardStack.Push(new BlockedNode(item));
            }
        }

        public async Task<INavigationNode?> MovePreviousAsync(INavigationBlock block, INavigationContext context)
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

        private static INavigationNode? GetValidPreviousNode(INavigationBlock block)
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
}
