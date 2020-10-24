namespace IS.Reading.StoryboardItems
{
    public class ProtagonistRewardItem : IStoryboardItem
    {
        public string Text { get; }

        public ProtagonistRewardItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Reward.Open(Text);
            return new ProtagonistAntiRewardItem(Text, Condition);
        }

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
