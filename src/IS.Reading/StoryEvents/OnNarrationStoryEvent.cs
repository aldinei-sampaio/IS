namespace IS.Reading.StoryEvents
{
    public struct OnNarrationStoryEvent : IClosableStoryEvent
    {
        public string Text { get; }

        public OnNarrationStoryEvent(string text)
            => Text = text;

        public IStoryEvent? Execute(IStoryContextUpdater context)
        {
            if (context.CurrentEvent == null)
            {
                context.CallOnNarrationOpen();
            }
            else
            {
                if (context.CurrentEvent.GetType() != typeof(OnNarrationStoryEvent))
                {
                    context.CurrentEvent.Close(context);
                    context.CallOnNarrationOpen();
                }
            }

            var oldEvent = context.CurrentEvent;
            context.CurrentEvent = this;
            context.CallOnNarrationChange(Text);
            return oldEvent;
        }

        public void Close(IStoryContextUpdater context) => context.CallOnNarrationClose();
    }
}
