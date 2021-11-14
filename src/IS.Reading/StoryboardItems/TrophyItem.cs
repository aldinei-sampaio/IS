namespace IS.Reading.StoryboardItems;

public struct TrophyItem : IStoryboardItem
{
    public Trophy Trophy { get; }

    public TrophyItem(Trophy trophy, ICondition? condition)
        => (Trophy, Condition) = (trophy, condition);

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        await context.Trophy.OpenAsync(Trophy);
        return new AntiTrophyItem(Trophy, Condition);
    }

    public ICondition? Condition { get; }
}
