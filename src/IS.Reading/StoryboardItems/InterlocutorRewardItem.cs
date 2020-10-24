namespace IS.Reading.StoryboardItems
{
    public class InterlocutorRewardItem : IStoryboardItem
    {
        public string Text { get; }

        public InterlocutorRewardItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Interlocutor.Reward.Open(Text);
            return new InterlocutorAntiRewardItem(Text, Condition);
        }

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
