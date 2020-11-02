namespace IS.Reading.StoryboardItems
{
    public struct InterlocutorSpeechTextItem : IStoryboardItem
    {
        public string Text { get; }

        public InterlocutorSpeechTextItem(string text, ICondition? condition)
            => (Text, Condition) = (text, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Interlocutor.Speech.Change(Text);
            return this;
        }

        public void Leave(IStoryContextEventCaller context) { }

        public bool IsPause => true;

        public ICondition? Condition { get; }
    }
}
