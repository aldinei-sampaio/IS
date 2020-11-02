namespace IS.Reading.StoryboardItems
{
    public class PromptItem : IStoryboardItem
    {
        public Prompt Prompt { get; }

        public PromptItem(Prompt prompt, ICondition? condition)
        {
            Prompt = prompt;
            Condition = condition;
            Block = new StoryboardBlock(this);
        }            

        public StoryboardBlock Block { get; }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Prompt.Open(Prompt);
            return this;
        }

        public ICondition? Condition { get; }
    }
}
