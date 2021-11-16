namespace IS.Reading.StoryboardItems;

public struct TutorialTextItem : IStoryboardItem
{
    public string Text { get; }

    public TutorialTextItem(string text, ICondition? condition)
        => (Text, Condition) = (text, condition);

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Tutorial.ChangeAsync(Text);

        if (context.CurrentItem is TutorialTextItem item)
            return context.CurrentItem;

        return null;
    }

    public bool IsPause => true;

    public ICondition? Condition { get; }
}
