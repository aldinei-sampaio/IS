namespace IS.Reading.StoryboardItems
{
    public struct ProtagonistThoughtItem : IStoryboardItem
    {
        public ProtagonistThoughtItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock();
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Thought.Open();
            return this;
        }

        public void Leave(IStoryContextEventCaller context)
            => context.Protagonist.Thought.Close();

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
