namespace IS.Reading.StoryboardItems;

public class InterlocutorBumpItem : IStoryboardItem
{
    public InterlocutorBumpItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Interlocutor.BumpAsync();
        return this;
    }

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
