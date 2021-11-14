namespace IS.Reading.StoryboardItems;

public class InterlocutorAntiRewardItem : IStoryboardItem
{
    public VarIncrement Increment { get; }

    public InterlocutorAntiRewardItem(VarIncrement increment, ICondition? condition)
        => (Increment, Condition) = (increment, condition);

    public Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        context.Variables[Increment.Name] = context.Variables[Increment.Name] - Increment.Value;
        return Task.FromResult<IStoryboardItem>(new InterlocutorRewardItem(Increment, Condition));
    }

    public ICondition? Condition { get; }
}
