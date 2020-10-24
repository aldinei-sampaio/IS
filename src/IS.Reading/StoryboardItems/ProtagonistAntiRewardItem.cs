namespace IS.Reading.StoryboardItems
{
    public class ProtagonistAntiRewardItem : IStoryboardItem
    {
        public string Text { get; }

        public ProtagonistAntiRewardItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
            => new ProtagonistRewardItem(Text, Condition);

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
