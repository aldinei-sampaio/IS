namespace IS.Reading.StoryboardItems
{
    public class ProtagonistThoughtItem : IStoryboardItem
    {
        public ProtagonistThoughtItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Thought.Open();
            return this;
        }

        public void Leave(IStoryContextEventCaller context)
            => context.Protagonist.Thought.Close();

        public StoryboardBlock Block { get; }

        public ICondition? Condition { get; }
    }
}
