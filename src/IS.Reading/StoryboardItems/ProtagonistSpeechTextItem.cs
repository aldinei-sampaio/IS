namespace IS.Reading.StoryboardItems
{
    public struct ProtagonistSpeechTextItem : IStoryboardItem
    {
        public string Text { get; }

        public ProtagonistSpeechTextItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Speech.Change(Text);
            return this;
        }

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => true;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
