namespace IS.Reading.StoryboardItems
{
    public class ProtagonistVoiceItem : IStoryboardItem
    {
        public ProtagonistVoiceItem(ICondition? condition) => Condition = condition;

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            context.CallOnProtagonistSpeakOpen();
            return this;
        }

        public void Leave(IStoryContextUpdater context) => context.CallOnProtagonistSpeakClose();

        public StoryboardBlock Block { get; } = new StoryboardBlock();

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
