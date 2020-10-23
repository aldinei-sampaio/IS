namespace IS.Reading.StoryboardItems
{
    public class ProtagonistEmotionItem : IStoryboardItem
    {
        public string Name { get; }

        public ProtagonistEmotionItem(string name, ICondition? condition)
            => (Name, Condition) = (name, condition);

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            context.CallOnProtagonistFeelingChange(Name);
            return this;
        }

        public void Leave(IStoryContextUpdater context) { }

        public StoryboardBlock Block { get; } = new StoryboardBlock();

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
