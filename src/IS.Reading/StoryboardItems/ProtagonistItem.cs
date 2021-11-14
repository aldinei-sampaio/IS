namespace IS.Reading.StoryboardItems;

public class ProtagonistItem : IStoryboardItem
{
    public ProtagonistItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Protagonist.EnterAsync(context.State[Keys.Protagonist]);
        return this;
    }

    public async Task LeaveAsync(IStoryContextEventCaller context)
        => await context.Protagonist.LeaveAsync();

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
