namespace IS.Reading.StoryboardItems
{
    public class InterlocutorItem : IStoryboardItem
    {
        public string Name { get; }

        public InterlocutorItem(string name, ICondition? condition)
        {
            Name = name;
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Interlocutor.Enter(Name);
            return this;
        }

        public void Leave(IStoryContextEventCaller context)
            => context.Interlocutor.Leave();

        public StoryboardBlock Block { get; }

        public ICondition? Condition { get; }
    }
}
