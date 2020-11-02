namespace IS.Reading.StoryboardItems
{
    public struct PauseItem : IStoryboardItem
    {
        public PauseItem(ICondition? condition) => Condition = condition;

        public IStoryboardItem Enter(IStoryContextEventCaller context) => this;

        public bool IsPause => true;

        public ICondition? Condition { get; }
    }
}
