namespace IS.Reading.StoryboardItems
{
    public struct TutorialTextItem : IStoryboardItem
    {
        public string Text { get; }

        public TutorialTextItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Tutorial.Change(Text);
            return this;
        }

        public bool IsPause => true;

        public ICondition? Condition { get; }
    }
}
