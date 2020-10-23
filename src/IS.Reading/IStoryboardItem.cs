namespace IS.Reading
{
    public interface IStoryboardItem
    {
        IStoryboardItem Enter(IStoryContextUpdater context);
        StoryboardBlock? Block { get; }
        void Leave(IStoryContextUpdater context);
        bool IsPause { get; }
        bool AllowBackwardsBlockEntry { get; }
        ICondition? Condition { get; }
    }
}
