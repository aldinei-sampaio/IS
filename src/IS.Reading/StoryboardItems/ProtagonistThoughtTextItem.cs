namespace IS.Reading.StoryboardItems
{
    public struct ProtagonistThoughtTextItem : IStoryboardItem
    {
        public string Text { get; }

        public ProtagonistThoughtTextItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Thought.Change(Text);
            return this;
        }

        public bool IsPause => true;

        public ICondition? Condition { get; }
    }
}
