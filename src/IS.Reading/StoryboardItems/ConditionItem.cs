namespace IS.Reading.StoryboardItems
{
    public class ConditionItem : IStoryboardItem
    {
        public ConditionItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context) => this;

        public StoryboardBlock Block { get; }

        public ICondition? Condition { get; }
    }
}
