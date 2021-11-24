namespace IS.Reading.Navigation;

public interface INode
{
    Task<INode> EnterAsync(INavigationContext context);
    Task LeaveAsync(INavigationContext context);
    ICondition? When { get; }
    ICondition? While { get; }
    IBlock? ChildBlock { get; }
}
