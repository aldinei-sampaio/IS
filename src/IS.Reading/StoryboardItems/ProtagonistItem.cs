namespace IS.Reading.StoryboardItems
{
    public struct ProtagonistItem : IStoryboardItem
    {
        public string Name { get; }

        public ProtagonistItem(string name, ICondition? condition)
        {
            Name = name;
            Condition = condition;
            Block = new StoryboardBlock();
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Protagonist.Enter(Name);
            return this;
        }

        public void Leave(IStoryContextEventCaller context) 
            => context.Protagonist.Leave();

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
