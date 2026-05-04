using IS.Reading.Conditions;

namespace IS.Reading.Choices;

public class BuilderDecisionItem<T>(ICondition condition, IEnumerable<IBuilder<T>> block) : IBuilderDecisionItem<T>
{
    public ICondition Condition { get; } = condition;
    public IEnumerable<IBuilder<T>> Block { get; } = block;
}
