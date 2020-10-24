namespace IS.Reading.StoryboardItems
{
    public struct InterlocutorSpeechItem : IStoryboardItem
    {
        public InterlocutorSpeechItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock();
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Speech.Open();
            return this;
        }

        public void Leave(IStoryContextEventCaller context)
            => context.Protagonist.Speech.Close();

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
