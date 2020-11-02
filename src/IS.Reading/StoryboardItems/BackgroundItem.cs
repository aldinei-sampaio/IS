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

        public ICondition? Condition { get; }

        public bool ChangesContext => true;

        public void OnStoryboardFinish(IStoryContextEventCaller context)
        {
            if (context.State[Keys.BackgroundImage] != string.Empty)
            {
                context.State[Keys.BackgroundImage] = string.Empty;
                context.Background.Change(string.Empty);
            }
        }
    }
}
