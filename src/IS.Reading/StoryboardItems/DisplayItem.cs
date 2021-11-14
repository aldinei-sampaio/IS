namespace IS.Reading.StoryboardItems;

public struct DisplayItem : IStoryboardItem
{
    public Display Display { get; }

    public DisplayItem(Display display, ICondition condition)
        => (Display, Condition) = (display, condition);

    public ICondition? Condition { get; }

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Display.OpenAsync(Display);
        return this;
    }
}
