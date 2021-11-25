namespace IS.Reading.Navigation;

public interface INode
{
    Task<INode> EnterAsync(INavigationContext context);
    Task LeaveAsync(INavigationContext context) => Task.CompletedTask;
    ICondition? When => null;
    ICondition? While => null;
    IBlock? ChildBlock => null;
}
