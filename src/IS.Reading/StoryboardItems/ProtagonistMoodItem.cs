﻿namespace IS.Reading.StoryboardItems
{
    public class ProtagonistMoodItem : IStoryboardItem
    {
        public string Name { get; }

        public ProtagonistMoodItem(string name, ICondition? condition)
        {
            Name = name;
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Mood.Change(Name);
            return this;
        }

        public StoryboardBlock Block { get; }

        public ICondition? Condition { get; }
    }
}
