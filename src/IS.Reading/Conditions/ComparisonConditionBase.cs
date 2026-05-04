using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public abstract class ComparisonConditionBase(IConditionKeyword left, IConditionKeyword right) : WritableBase, ICondition
{
    public IConditionKeyword Left { get; } = left;
    public IConditionKeyword Right { get; } = right;

    public abstract bool Evaluate(IVariableDictionary variables);
}
