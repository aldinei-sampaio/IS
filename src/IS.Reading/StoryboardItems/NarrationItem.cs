﻿namespace IS.Reading.StoryboardItems
{
    public struct NarrationItem : IStoryboardItem
    {
        public NarrationItem(ICondition? condition)
        {
            Condition = condition;
            Block = new StoryboardBlock();
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
