namespace IS.Reading.StoryboardItems
{
    public class InterlocutorAntiRewardItem : IStoryboardItem
    {
        public VarIncrement Increment { get; }

        public InterlocutorAntiRewardItem(VarIncrement increment, ICondition? condition)
            => (Increment, Condition) = (increment, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Variables[Increment.Name] = context.Variables[Increment.Name] - Increment.Value;
            return new InterlocutorRewardItem(Increment, Condition);
        }

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
