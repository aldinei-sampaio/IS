namespace IS.Reading.StoryboardItems;

public struct NarrationTextItem : IStoryboardItem
{
    public string Text { get; }

    public NarrationTextItem(string text, ICondition? condition)
        => (Text, Condition) = (text, condition);

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Narration.ChangeAsync(Text);
        return this;
    }

    public bool IsPause => true;

    public ICondition? Condition { get; }
}
