namespace IS.Reading.StoryboardItems
{
    public class PromptItem : IStoryboardItem
    {
        public Prompt Prompt { get; }

        public PromptItem(Prompt prompt, ICondition? condition)
            => (Prompt, Condition) = (prompt, condition);

        public StoryboardBlock Block { get; } = new StoryboardBlock();

        public bool IsPause => false;

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            context.CallOnPromptOpen(Prompt);
            return this;
        }

        public void Leave(IStoryContextUpdater context) { }

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
