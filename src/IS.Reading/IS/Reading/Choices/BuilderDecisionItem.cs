using IS.Reading.Conditions;

namespace IS.Reading.Choices;

public class BuilderDecisionItem<T> : IBuilderDecisionItem<T>
{
    public ICondition Condition { get; }
    public IEnumerable<IBuilder<T>> Block { get; }

    public BuilderDecisionItem(ICondition condition, IEnumerable<IBuilder<T>> block)
    {
        Condition = condition;
        Block = block;
    }
}
