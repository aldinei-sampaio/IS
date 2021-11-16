namespace IS.Reading.Navigation
{
    public interface INavigationNode
    {
        Task<INavigationNode> EnterAsync(INavigationContext context);
        Task LeaveAsync(INavigationContext context);
        ICondition? Condition { get; }
    }
}
