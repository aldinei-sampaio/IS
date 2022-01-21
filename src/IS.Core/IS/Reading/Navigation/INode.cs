using IS.Reading.Conditions;

namespace IS.Reading.Navigation;

public interface INode
{
    Task<object?> EnterAsync(INavigationContext context) => Task.FromResult((object?)null);
    Task EnterAsync(INavigationContext context, object? state) => EnterAsync(context);
    Task LeaveAsync(INavigationContext context) => Task.CompletedTask;
    ICondition? When => null;
    IBlock? ChildBlock => null;
}
