namespace IS.Reading.StoryboardItems
{
    public struct ProtagonistMoodItem : IStoryboardItem
    {
        public string Name { get; }

        public ProtagonistMoodItem(string name, ICondition? condition)
        {
            Name = name;
            Condition = condition;
            Block = new StoryboardBlock();
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Mood.Change(Name);
            return this;
        }

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
