namespace IS.Reading.StoryboardItems;

public class InterlocutorSpeechItem : IStoryboardItem
{
    public InterlocutorSpeechItem(ICondition? condition)
    {
        Condition = condition;
        Block = new StoryboardBlock(this);
    }

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Interlocutor.Speech.OpenAsync();
        return this;
    }

    public async Task LeaveAsync(IStoryContextEventCaller context)
        => await context.Interlocutor.Speech.CloseAsync();

    public StoryboardBlock Block { get; }

    public ICondition? Condition { get; }
}
