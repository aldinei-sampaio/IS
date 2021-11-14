namespace IS.Reading.StoryboardItems;

public class NarrationItem : IStoryboardItem
{
    public NarrationItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Narration.OpenAsync();
        return this;
    }

    public async Task LeaveAsync(IStoryContextEventCaller context)
        => await context.Narration.CloseAsync();

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
