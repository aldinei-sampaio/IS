namespace IS.Reading.Navigation
{
    public interface INode
    {
        Task<INode> EnterAsync(IContext context);
        Task LeaveAsync(IContext context);
        ICondition? Condition { get; }
        IBlock? ChildBlock { get; }
    }
}
