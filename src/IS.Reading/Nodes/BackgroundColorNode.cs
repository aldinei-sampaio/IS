using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public struct BackgroundColorNode : INode
    {
        public string Color { get; }

        public BackgroundColorNode(string color, ICondition? condition)
            => (Color, Condition) = (color, condition);

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
