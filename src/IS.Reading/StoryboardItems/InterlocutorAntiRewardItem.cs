namespace IS.Reading.StoryboardItems
{
    public class InterlocutorAntiRewardItem : IStoryboardItem
    {
        public string Text { get; }

        public InterlocutorAntiRewardItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
            => new InterlocutorRewardItem(Text, Condition);

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
