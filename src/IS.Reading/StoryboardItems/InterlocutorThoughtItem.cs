namespace IS.Reading.StoryboardItems
{
    public class InterlocutorThoughtItem : IStoryboardItem
    {
        public InterlocutorThoughtItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Interlocutor.Thought.Open();
            return this;
        }

        public void Leave(IStoryContextEventCaller context)
            => context.Interlocutor.Thought.Close();

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
