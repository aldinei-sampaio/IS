namespace IS.Reading.StoryboardItems;

public struct PauseItem : IStoryboardItem
{
    public PauseItem(ICondition? condition) => Condition = condition;

    public Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context) 
        => Task.FromResult<IStoryboardItem?>(this);

    public bool IsPause => true;

    public ICondition? Condition { get; }
}
