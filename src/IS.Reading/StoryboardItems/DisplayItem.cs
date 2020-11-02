namespace IS.Reading.StoryboardItems
{
    public struct DisplayItem : IStoryboardItem
    {
        public Display Display { get; }

        public DisplayItem(Display display, ICondition condition)
            => (Display, Condition) = (display, condition);

        public ICondition? Condition { get; }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Display.Open(Display);
            return this;
        }
    }
}
