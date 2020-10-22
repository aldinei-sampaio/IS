namespace IS.Reading.StoryEvents
{
    public struct BackgroundStoryEvent : IStoryEvent
    {
        public const string ValueKey = "BackgroundImage";

        public string ImageName { get; }

        public BackgroundStoryEvent(string imageName)
            => ImageName = imageName;

        public IStoryEvent? Execute(IStoryContextUpdater context)
        {
            var oldValue = (string?)context[ValueKey] ?? string.Empty;
            if (oldValue.EqualsCI(ImageName))
                return null;

            context.CurrentEvent?.Close(context);

            context[ValueKey] = ImageName;
            context.CallOnBackgroundChange(ImageName);
            return new BackgroundStoryEvent(oldValue);
        }
    }
}
