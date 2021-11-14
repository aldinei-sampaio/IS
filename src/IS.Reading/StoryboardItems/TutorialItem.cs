namespace IS.Reading.StoryboardItems;

public class TutorialItem : IStoryboardItem
{
    public TutorialItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Tutorial.OpenAsync();
        return this;
    }

    public async Task LeaveAsync(IStoryContextEventCaller context)
        => await context.Tutorial.CloseAsync();

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
