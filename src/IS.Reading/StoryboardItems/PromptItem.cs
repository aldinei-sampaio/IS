namespace IS.Reading.StoryboardItems
{
    public struct PromptItem : IStoryboardItem
    {
        public Prompt Prompt { get; }

        public PromptItem(Prompt prompt, ICondition? condition)
        {
            Prompt = prompt;
            Condition = condition;
            Block = new StoryboardBlock();
        }            

        public StoryboardBlock Block { get; }

        public bool IsPause => false;

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Prompt.Open(Prompt);
            return this;
        }

        public void Leave(IStoryContextEventCaller context) { }

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
