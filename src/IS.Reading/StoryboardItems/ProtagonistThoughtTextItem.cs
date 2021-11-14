namespace IS.Reading.StoryboardItems;

public struct ProtagonistThoughtTextItem : IStoryboardItem
{
    public string Text { get; }

    public ProtagonistThoughtTextItem(string text, ICondition? condition)
        => (Text, Condition) = (text, condition);

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Protagonist.Thought.ChangeAsync(Text);
        return this;
    }

    public bool IsPause => true;

    public ICondition? Condition { get; }
}
