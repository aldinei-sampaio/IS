namespace IS.Reading.StoryEvents
{
    public struct MusicStoryEvent : IStoryEvent
    {
        public const string ValueKey = "Music";

        public string MusicName { get; }

        public MusicStoryEvent(string musicName)
            => MusicName = musicName;

        public IStoryEvent? Execute(IStoryContextUpdater context)
        {
            var oldValue = (string?)context[ValueKey] ?? string.Empty;
            if (oldValue.EqualsCI(MusicName))
                return null;

            context[ValueKey] = MusicName;
            context.CallOnMusicChange(MusicName);
            return new MusicStoryEvent(oldValue);
        }
    }
}
