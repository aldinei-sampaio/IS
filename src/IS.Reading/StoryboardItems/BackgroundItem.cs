namespace IS.Reading.StoryboardItems;

public struct BackgroundItem : IStoryboardItem
{
    public string ImageName { get; }

    public BackgroundItem(string imageName, ICondition? condition)
        => (ImageName, Condition) = (imageName, condition);

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        var oldValue = context.State.Set(Keys.BackgroundImage, ImageName);
        await context.Background.ChangeAsync(ImageName);
        return new BackgroundItem(oldValue, Condition);
    }

    public ICondition? Condition { get; }

    public bool ChangesContext => true;

    public async Task OnStoryboardFinishAsync(IStoryContextEventCaller context)
    {
        if (context.State[Keys.BackgroundImage] != string.Empty)
        {
            context.State[Keys.BackgroundImage] = string.Empty;
            await context.Background.ChangeAsync(string.Empty);
        }
    }
}
