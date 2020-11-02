namespace IS.Reading.StoryboardItems
{
    public struct NarrationTextItem : IStoryboardItem
    {
        public string Text { get; }

        public NarrationTextItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Narration.Change(Text);
            return this;
        }

        public bool IsPause => true;

        public ICondition? Condition { get; }
    }
}
