namespace IS.Reading.StoryboardItems
{
    public struct VarIncrementItem : IStoryboardItem
    {
        public VarIncrement Increment { get; }

        public VarIncrementItem(VarIncrement increment, ICondition? condition)
            => (Increment, Condition) = (increment, condition);

        public IStoryboardItem Enter(IStoryContextEventCaller context)
        {
            context.Variables[Increment.Name] = context.Variables[Increment.Name] + Increment.Value;
            return new VarIncrementItem(new VarIncrement(Increment.Name, -Increment.Value), Condition);
        }

        public ICondition? Condition { get; }
    }
}
