﻿using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class BlockNode : INode
    {
        public BlockNode(IBlock childBlock, ICondition? when, ICondition? @while)
            => (ChildBlock, When, While) = (childBlock, when, @while);

        public ICondition? When { get; }

        public ICondition? While { get; }

        public IBlock? ChildBlock { get; }

        public Task<INode> EnterAsync(INavigationContext context)
        {
            throw new NotImplementedException();
        }

        public Task LeaveAsync(INavigationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
