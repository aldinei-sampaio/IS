namespace IS.Reading.StoryboardItems
{
    public class TutorialItem : IStoryboardItem
    {
        public TutorialItem(ICondition? condition) => Condition = condition;        

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            context.CallOnTutorialOpen();
            return this;
        }

        public void Leave(IStoryContextUpdater context) => context.CallOnTutorialClose();

        public StoryboardBlock Block { get; } = new StoryboardBlock();

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
