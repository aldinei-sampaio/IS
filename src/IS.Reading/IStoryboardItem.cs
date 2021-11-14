namespace IS.Reading;

public interface IStoryboardItem
{
    Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context);
    StoryboardBlock? Block => null;
    Task LeaveAsync(IStoryContextEventCaller context) => Task.CompletedTask;
    bool IsPause => false;
    bool AllowBackwardsBlockEntry => true;
    ICondition? Condition => null;
    bool ChangesContext => false;
    Task OnStoryboardFinishAsync(IStoryContextEventCaller context) => Task.CompletedTask;
}
