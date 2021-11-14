namespace IS.Reading.StoryboardItems;

public class InterlocutorThoughtItem : IStoryboardItem
{
    public InterlocutorThoughtItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Interlocutor.Thought.OpenAsync();
        return this;
    }

    public async Task LeaveAsync(IStoryContextEventCaller context)
        => await context.Interlocutor.Thought.CloseAsync();

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
