﻿namespace IS.Reading.StoryboardItems
{
    public struct TutorialItem : IStoryboardItem
    {
        public TutorialItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock();
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Tutorial.Open();
            return this;
        }

        public void Leave(IStoryContextEventCaller context)
            => context.Tutorial.Close();

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
