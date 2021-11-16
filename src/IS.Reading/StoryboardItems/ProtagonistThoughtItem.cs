namespace IS.Reading.StoryboardItems;

public class ProtagonistThoughtItem : IStoryboardItem
{
    public ProtagonistThoughtItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Protagonist.Thought.OpenAsync();
        return this;
    }

    public async Task LeaveAsync(IStoryContextEventCaller context)
        => await context.Protagonist.Thought.CloseAsync();

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
