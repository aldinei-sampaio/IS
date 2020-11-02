namespace IS.Reading.StoryboardItems
{
    public class InterlocutorRewardItem : IStoryboardItem
    {
        public VarIncrement Increment { get; }

        public InterlocutorRewardItem(VarIncrement increment, ICondition? condition)
            => (Increment, Condition) = (increment, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Variables[Increment.Name] = context.Variables[Increment.Name] + Increment.Value;
            context.Interlocutor.Reward.Open(Increment);
            return new InterlocutorAntiRewardItem(Increment, Condition);
        }

        public ICondition? Condition { get; }
    }
}
