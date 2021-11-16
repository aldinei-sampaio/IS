namespace IS.Reading.StoryboardItems;

public struct AntiTrophyItem : IStoryboardItem
{
    public Trophy Trophy { get; }

    public AntiTrophyItem(Trophy trophy, ICondition? condition)
        => (Trophy, Condition) = (trophy, condition);

    public Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
        => Task.FromResult<IStoryboardItem?>(new TrophyItem(Trophy, Condition));

    public ICondition? Condition { get; }
}
