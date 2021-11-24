using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class BackgroundRightNode : INode
    {
        public string ImageName { get; }

        public BackgroundRightNode(string imageName, ICondition? condition)
            => (ImageName, When) = (imageName, condition);

        public ICondition? When { get; }

        public ICondition? While => null;

        public IBlock? ChildBlock => null;

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
