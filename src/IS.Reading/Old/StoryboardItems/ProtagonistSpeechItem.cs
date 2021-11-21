namespace IS.Reading.StoryboardItems;

public class ProtagonistSpeechItem : IStoryboardItem
{
    public ProtagonistSpeechItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Protagonist.Speech.OpenAsync();
        return this;
    }

    public async Task LeaveAsync(IStoryContextEventCaller context)
        => await context.Protagonist.Speech.CloseAsync();

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
