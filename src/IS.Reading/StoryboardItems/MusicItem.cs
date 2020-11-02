namespace IS.Reading.StoryboardItems
{
    public struct MusicItem : IStoryboardItem
    {
        public string MusicName { get; }

        public MusicItem(string musicName, ICondition? condition)
            => (MusicName, Condition) = (musicName, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            var oldValue = context.State.Set(Keys.Music, MusicName);
            context.Music.Change(MusicName);
            return new MusicItem(oldValue, Condition);
        }

        public ICondition? Condition { get; }

        public bool ChangesContext => true;

        public void OnStoryboardFinish(IStoryContextEventCaller context)
        {
            if (context.State[Keys.Music] != string.Empty)
            {
                context.State[Keys.Music] = string.Empty;
                context.Music.Change(string.Empty);
            }
        }
    }
}
