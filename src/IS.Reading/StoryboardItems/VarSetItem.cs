namespace IS.Reading.StoryboardItems
{
    public struct VarSetItem : IStoryboardItem
    {
        public string Name { get; }

        public int Value { get; }

        public VarSetItem(string name, int value, ICondition? condition)
            => (Name, Value, Condition) = (name, value, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            var oldValue = context.Variables.Set(Name, Value);
            return new VarSetItem(Name, oldValue, Condition);
        }

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
