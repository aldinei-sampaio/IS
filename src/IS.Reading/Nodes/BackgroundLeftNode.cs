using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class BackgroundLeftNode : INode
    {
        public string ImageName { get; }

        public BackgroundLeftNode(string imageName, ICondition? condition)
            => (ImageName, When) = (imageName, condition);

        public ICondition? When { get; }

        public ICondition? While => null;

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
