namespace IS.Reading.StoryboardItems
{
    public struct TrophyItem : IStoryboardItem
    {
        public Trophy Trophy { get; }

        public TrophyItem(Trophy trophy, ICondition? condition)
            => (Trophy, Condition) = (trophy, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Trophy.Open(Trophy);
            return new AntiTrophyItem(Trophy, Condition);
        }

        public ICondition? Condition { get; }
    }
}
