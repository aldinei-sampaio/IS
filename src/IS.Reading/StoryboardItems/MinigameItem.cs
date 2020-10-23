namespace IS.Reading.StoryboardItems
{
    public class MinigameItem : IStoryboardItem
    {
        public MinigameItem(ICondition? condition) => Condition = condition;
        
        public IStoryboardItem Enter(IStoryContextUpdater context) => this;

        public void Leave(IStoryContextUpdater context) { }

        public StoryboardBlock? Block { get; } = new StoryboardBlock();

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => false;

        public ICondition? Condition { get; }
    }
}
