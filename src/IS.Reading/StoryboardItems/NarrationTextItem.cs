namespace IS.Reading.StoryboardItems
{
    public class NarrationTextItem : IStoryboardItem
    {
        public string Text { get; }

        public NarrationTextItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            context.CallOnNarrationChange(Text);
            return this;
        }

        public void Leave(IStoryContextUpdater context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => true;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
