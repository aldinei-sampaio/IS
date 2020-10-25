namespace IS.Reading.StoryboardItems
{
    public struct ProtagonistChangeItem : IStoryboardItem
    {
        public string Name { get; }

        public ProtagonistChangeItem(string name, ICondition? condition)
            => (Name, Condition) = (name, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            var oldValue = context.State.Set(Keys.Protagonist, Name);
            context.Protagonist.Change(Name);
            return new ProtagonistChangeItem(oldValue, Condition);
        }

        public void Leave(IStoryContextEventCaller context) { }

        public StoryboardBlock? Block => null;

        public bool IsPause => false;

        public bool AllowBackwardsBlockEntry => true;

        public ICondition? Condition { get; }
    }
}
