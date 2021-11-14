namespace IS.Reading.StoryboardItems;

public class InterlocutorRewardItem : IStoryboardItem
{
    public VarIncrement Increment { get; }

    public InterlocutorRewardItem(VarIncrement increment, ICondition? condition)
        => (Increment, Condition) = (increment, condition);

    public async Task<IStoryboardItem> EnterAsync(IStoryContextEventCaller context)
    {
        context.Variables[Increment.Name] = context.Variables[Increment.Name] + Increment.Value;
        await context.Interlocutor.Reward.OpenAsync(Increment);
        return new InterlocutorAntiRewardItem(Increment, Condition);
    }

    public ICondition? Condition { get; }
}
