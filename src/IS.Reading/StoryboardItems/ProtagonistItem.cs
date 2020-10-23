namespace IS.Reading.StoryboardItems
{
    public class ProtagonistItem : IStoryboardItem
    {
        public string Name { get; }

        public ProtagonistItem(string name, ICondition? condition)
            => (Name, Condition) = (name, condition);

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            context.CallOnProtagonistArrive(Name);
            return this;
        }

        public void Leave(IStoryContextUpdater context) => context.CallOnProtagonistLeave();

        public StoryboardBlock Block { get; } = new StoryboardBlock();

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
