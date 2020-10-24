namespace IS.Reading.StoryboardItems
{
    public struct BackgroundItem : IStoryboardItem
    {
        public string ImageName { get; }

        public BackgroundItem(string imageName, ICondition? condition)
            => (ImageName, Condition) = (imageName, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            var oldValue = context.State.Set(Keys.BackgroundImage, ImageName);
            context.Background.Change(ImageName);
            return new BackgroundItem(oldValue, Condition);
        }

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
