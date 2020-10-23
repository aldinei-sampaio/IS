namespace IS.Reading.StoryboardItems
{
    public class BackgroundItem : IStoryboardItem
    {
        public string ImageName { get; }

        public BackgroundItem(string imageName, ICondition? condition)
            => (ImageName, Condition) = (imageName, condition);

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            var oldValue = context.SetValue(Keys.BackgroundImage, ImageName);
            context.CallOnBackgroundChange(ImageName);
            return new BackgroundItem(oldValue, Condition);
        }

        public void Leave(IStoryContextUpdater context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
