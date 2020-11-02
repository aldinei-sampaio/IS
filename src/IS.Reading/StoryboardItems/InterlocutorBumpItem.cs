namespace IS.Reading.StoryboardItems
{
    public class InterlocutorBumpItem : IStoryboardItem
    {
        public InterlocutorBumpItem(ICondition? condition)
        { 
            Condition = condition;
            Block = new StoryboardBlock(this);
        }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Interlocutor.Bump();
            return this;
        }

        public StoryboardBlock Block { get; }

        public ICondition? Condition { get; }
    }
}
