﻿namespace IS.Reading.StoryboardItems
{
    public class ProtagonistBumpItem : IStoryboardItem
    {
        public ProtagonistBumpItem(ICondition? condition)
        { 
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Bump();
            return this;
        }

        public StoryboardBlock Block { get; }

        public ICondition? Condition { get; }
    }
}
