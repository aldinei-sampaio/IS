namespace IS.Reading.StoryboardItems;

public class MinigameItem : IStoryboardItem
{
    public MinigameItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
        => Task.FromResult<IStoryboardItem>(this);

    public StoryboardBlock? Block { get; }

    public bool IsPause => false;

    public bool AllowBackwardsBlockEntry => false;

    public ICondition? Condition { get; }
}
