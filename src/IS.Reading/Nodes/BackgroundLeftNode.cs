﻿using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public struct BackgroundLeftNode : INode
    {
        public string ImageName { get; }

        public BackgroundLeftNode(string imageName, ICondition? condition)
            => (ImageName, Condition) = (imageName, condition);

        public ICondition? Condition { get; }

        public IBlock? ChildBlock => null;

        public Task<INode> EnterAsync(IContext context)
        {
            throw new NotImplementedException();
        }

        public Task LeaveAsync(IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
