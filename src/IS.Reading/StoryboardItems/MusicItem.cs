﻿namespace IS.Reading.StoryboardItems
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

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
