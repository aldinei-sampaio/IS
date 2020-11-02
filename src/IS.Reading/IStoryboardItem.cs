namespace IS.Reading
{
    public interface IStoryboardItem
    {
        IStoryboardItem Enter(IStoryContextEventCaller context);
        StoryboardBlock? Block => null;
        void Leave(IStoryContextEventCaller context) { }
        bool IsPause => false;
        bool AllowBackwardsBlockEntry => true;
        ICondition? Condition => null;
        bool ChangesContext => false;
        void OnStoryboardFinish(IStoryContextEventCaller context) { }
    }
}
