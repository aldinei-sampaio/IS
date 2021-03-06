﻿namespace IS.Reading.StoryboardItems
{
    public class InterlocutorMoodItem : IStoryboardItem
    {
        public string Name { get; }

        public InterlocutorMoodItem(string name, ICondition? condition)
        {
            Name = name;
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Interlocutor.Mood.Change(Name);
            return this;
        }

        public StoryboardBlock Block { get; }

        public ICondition? Condition { get; }
    }
}
