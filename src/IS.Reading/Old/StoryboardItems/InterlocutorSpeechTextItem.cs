namespace IS.Reading.StoryboardItems;

public struct InterlocutorSpeechTextItem : IStoryboardItem
{
    public string Text { get; }

    public InterlocutorSpeechTextItem(string text, ICondition? condition)
        => (Text, Condition) = (text, condition);

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Interlocutor.Speech.ChangeAsync(Text);
        return this;
    }

    public bool IsPause => true;

    public ICondition? Condition { get; }
}
