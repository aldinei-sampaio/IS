namespace IS.Reading.StoryboardItems;

public struct InterlocutorThoughtTextItem : IStoryboardItem
{
    public string Text { get; }

    public InterlocutorThoughtTextItem(string text, ICondition? condition)
        => (Text, Condition) = (text, condition);

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Interlocutor.Thought.ChangeAsync(Text);
        return this;
    }

    public bool IsPause => true;

    public ICondition? Condition { get; }
}
