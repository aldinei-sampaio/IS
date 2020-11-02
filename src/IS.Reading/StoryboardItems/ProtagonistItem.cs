namespace IS.Reading.StoryboardItems
{
    public class ProtagonistItem : IStoryboardItem
    {
        public ProtagonistItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Enter(context.State[Keys.Protagonist]);
            return this;
        }

        public void Leave(IStoryContextEventCaller context) 
            => context.Protagonist.Leave();

        public StoryboardBlock Block { get; }

        public ICondition? Condition { get; }
    }
}
