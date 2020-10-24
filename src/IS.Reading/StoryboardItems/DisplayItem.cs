namespace IS.Reading.StoryboardItems
{
    public struct DisplayItem : IStoryboardItem
    {
        public Display Display { get; }

        public DisplayItem(Display display, ICondition condition)
            => (Display, Condition) = (display, condition);

        public StoryboardBlock? Block => null;

        public bool IsPause => true;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Display.Open(Display);
            return this;
        }

        public void Leave(IStoryContextEventCaller context) { }
    }
}
