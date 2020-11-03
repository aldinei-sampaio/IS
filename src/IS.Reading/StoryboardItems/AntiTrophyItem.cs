namespace IS.Reading.StoryboardItems
{
    public struct AntiTrophyItem : IStoryboardItem
    {
        public Trophy Trophy { get; }

        public AntiTrophyItem(Trophy trophy, ICondition? condition)
            => (Trophy, Condition) = (trophy, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
            => new TrophyItem(Trophy, Condition);

        public ICondition? Condition { get; }
    }
}
