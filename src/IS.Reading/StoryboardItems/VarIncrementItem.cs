namespace IS.Reading.StoryboardItems
{
    public struct VarIncrementItem : IStoryboardItem
    {
        public string Name { get; }

        public int Increment { get; }

        public VarIncrementItem(string name, int increment, ICondition? condition)
            => (Name, Increment, Condition) = (name, increment, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Variables[Name] = context.Variables[Name] + Increment;
            return new VarIncrementItem(Name, -Increment, Condition);
        }

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
