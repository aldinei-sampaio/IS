namespace IS.Reading.StoryboardItems;

public class ProtagonistBumpItem : IStoryboardItem
{
    public ProtagonistBumpItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Protagonist.BumpAsync();
        return this;
    }

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
