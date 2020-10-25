namespace IS.Reading.StoryboardItems
{
    public struct PauseItem : IStoryboardItem
    {
        public PauseItem(ICondition? condition)
            => Condition = condition;

        public IStoryboardItem Enter(IStoryContextEventCaller context)
            => this;

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => true;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
