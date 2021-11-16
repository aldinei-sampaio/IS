namespace IS.Reading.StoryboardItems;

public class ConditionItem : IStoryboardItem
{
    public ConditionItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
        => Task.FromResult<IStoryboardItem?>(this);

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
