namespace IS.Reading.StoryboardItems
{
    public class NarrationItem : IStoryboardItem
    {
        public NarrationItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Narration.Open();
            return this;
        }

        public void Leave(IStoryContextEventCaller context) 
            => context.Narration.Close();

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
