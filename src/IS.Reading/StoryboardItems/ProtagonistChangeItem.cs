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

        public ICondition? Condition { get; }

        public bool ChangesContext => true;

        public void OnStoryboardFinish(IStoryContextEventCaller context)
        {
            if (context.State[Keys.Protagonist] != string.Empty)
            {
                context.State[Keys.Protagonist] = string.Empty;
                context.Protagonist.Change(string.Empty);
            }
        }
    }
}
