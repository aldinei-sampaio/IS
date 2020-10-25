﻿namespace IS.Reading.StoryboardItems
{
    public class ConditionItem : IStoryboardItem
    {
        public ConditionItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context) => this;

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
