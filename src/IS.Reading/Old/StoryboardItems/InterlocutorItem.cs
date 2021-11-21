namespace IS.Reading.StoryboardItems;

public class InterlocutorItem : IStoryboardItem
{
    public string Name { get; }

    public InterlocutorItem(string name, ICondition? condition)
    {
        Name = name;
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Interlocutor.EnterAsync(Name);
        return this;
    }

    public async Task LeaveAsync(IStoryContextEventCaller context)
        => await context.Interlocutor.LeaveAsync();

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
