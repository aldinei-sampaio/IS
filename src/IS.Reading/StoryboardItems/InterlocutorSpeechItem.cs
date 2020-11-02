namespace IS.Reading.StoryboardItems
{
    public class InterlocutorSpeechItem : IStoryboardItem
    {
        public InterlocutorSpeechItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Interlocutor.Speech.Open();
            return this;
        }

        public void Leave(IStoryContextEventCaller context)
            => context.Interlocutor.Speech.Close();

        public StoryboardBlock Block { get; }

        public ICondition? Condition { get; }
    }
}
