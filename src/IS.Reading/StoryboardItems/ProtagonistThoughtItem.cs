namespace IS.Reading.StoryboardItems
{
    public class ProtagonistThoughtItem : IStoryboardItem
    {
        public ProtagonistThoughtItem(ICondition? condition) => Condition = condition;

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            context.CallOnProtagonistThoughtOpen();
            return this;
        }

        public void Leave(IStoryContextUpdater context) => context.CallOnProtagonistThoughtClose();

        public StoryboardBlock Block { get; } = new StoryboardBlock();

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
