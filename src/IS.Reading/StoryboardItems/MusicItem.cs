namespace IS.Reading.StoryboardItems;

public struct MusicItem : IStoryboardItem
{
    public string MusicName { get; }

    public MusicItem(string musicName, ICondition? condition)
        => (MusicName, Condition) = (musicName, condition);

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        var oldValue = context.State.Set(Keys.Music, MusicName);
        await context.Music.ChangeAsync(MusicName);
        return new MusicItem(oldValue, Condition);
    }

    public ICondition? Condition { get; }

    public bool ChangesContext => true;

    public async Task OnStoryboardFinishAsync(IStoryContextEventCaller context)
    {
        if (context.State[Keys.Music] != string.Empty)
        {
            context.State[Keys.Music] = string.Empty;
            await context.Music.ChangeAsync(string.Empty);
        }
    }
}
