namespace IS.Reading
{
    public interface IStoryboardItem
    {
        IStoryboardItem Enter(IStoryContextEventCaller context);
        StoryboardBlock? Block { get; }
        void Leave(IStoryContextEventCaller context);
        bool IsPause { get; }
        bool AllowBackwardsBlockEntry { get; }
        ICondition? Condition { get; }
    }
}
