namespace IS.Reading.StoryboardItems;

public struct ProtagonistChangeItem : IStoryboardItem
{
    public string Name { get; }

    public ProtagonistChangeItem(string name, ICondition? condition)
        => (Name, Condition) = (name, condition);

    public async Task<IStoryboardItem?> EnterAsync(IStoryContextEventCaller context)
    {
        var oldValue = context.State.Set(Keys.Protagonist, Name);
        await context.Protagonist.ChangeAsync(Name);
        return new ProtagonistChangeItem(oldValue, Condition);
    }

    public ICondition? Condition { get; }

    public bool ChangesContext => true;

    public async Task OnStoryboardFinishAsync(IStoryContextEventCaller context)
    {
        if (context.State[Keys.Protagonist] != string.Empty)
        {
            context.State[Keys.Protagonist] = string.Empty;
            await context.Protagonist.ChangeAsync(string.Empty);
        }
    }
}
