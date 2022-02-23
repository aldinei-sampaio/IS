using IS.Reading.Conditions;

namespace IS.Reading.Choices;

public interface IBuilderDecisionItem<T>
{
    public ICondition Condition { get; }
    public IEnumerable<IBuilder<T>> Block { get; }
}
