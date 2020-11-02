namespace IS.Reading.StoryboardItems
{
    public class ProtagonistAntiRewardItem : IStoryboardItem
    {
        public VarIncrement Increment { get; }

        public ProtagonistAntiRewardItem(VarIncrement increment, ICondition? condition)
            => (Increment, Condition) = (increment, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Variables[Increment.Name] = context.Variables[Increment.Name] - Increment.Value;
            return new ProtagonistRewardItem(Increment, Condition);
        }

        public ICondition? Condition { get; }
    }
}
