namespace IS.Reading.StoryboardItems
{
    public class MusicItem : IStoryboardItem
    {
        public string MusicName { get; }

        public MusicItem(string musicName, ICondition? condition)
            => (MusicName, Condition) = (musicName, condition);

        public IStoryboardItem Enter(IStoryContextUpdater context)
        {
            var oldValue = context.SetValue(Keys.Music, MusicName);
            context.CallOnMusicChange(MusicName);
            return new MusicItem(oldValue, Condition);
        }

        public void Leave(IStoryContextUpdater context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
