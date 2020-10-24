namespace IS.Reading.StoryboardItems
{
    public struct InterlocutorItem : IStoryboardItem
    {
        public string Name { get; }

        public InterlocutorItem(string name, ICondition? condition)
        {
            Name = name;
            Condition = condition;
            Block = new StoryboardBlock();
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Interlocutor.Enter(Name);
            return this;
        }

        public void Leave(IStoryContextEventCaller context)
            => context.Interlocutor.Leave();

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
