namespace IS.Reading.Navigation;

public interface INode
{
    Task<INode> EnterAsync(IContext context);
    Task LeaveAsync(IContext context);
    ICondition? When { get; }
    ICondition? While { get; }
    IBlock? ChildBlock { get; }
}
