namespace IS.Reading.StoryboardItems
{
    public class ProtagonistRewardItem : IStoryboardItem
    {
        public VarIncrement Increment { get; }

        public ProtagonistRewardItem(VarIncrement increment, ICondition? condition)
            => (Increment, Condition) = (increment, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Variables[Increment.Name] = context.Variables[Increment.Name] + Increment.Value;
            context.Protagonist.Reward.Open(Increment);
            return new ProtagonistAntiRewardItem(Increment, Condition);
        }

        public ICondition? Condition { get; }
    }
}
