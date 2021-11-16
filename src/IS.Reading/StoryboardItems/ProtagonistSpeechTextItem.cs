namespace IS.Reading.StoryboardItems;

public struct ProtagonistSpeechTextItem : IStoryboardItem
{
    public string Text { get; }

    public ProtagonistSpeechTextItem(string text, ICondition? condition)
        => (Text, Condition) = (text, condition);

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Protagonist.Speech.ChangeAsync(Text);
        return this;
    }

    public bool IsPause => true;

    public ICondition? Condition { get; }
}
