namespace IS.Reading.StoryboardItems
{
    public class NarrationItem : IStoryboardItem
    {
        public NarrationItem(ICondition? condition) => Condition = condition;

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            context.CallOnNarrationOpen();
            return this;
        }

        public void Leave(IStoryContextUpdater context) => context.CallOnNarrationClose();

        public StoryboardBlock Block { get; } = new StoryboardBlock();

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
