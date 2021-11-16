namespace IS.Reading.StoryboardItems;

public class ProtagonistMoodItem : IStoryboardItem
{
    public string Name { get; }

    public ProtagonistMoodItem(string name, ICondition? condition)
    {
        Name = name;
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Protagonist.Mood.ChangeAsync(Name);
        return this;
    }

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
