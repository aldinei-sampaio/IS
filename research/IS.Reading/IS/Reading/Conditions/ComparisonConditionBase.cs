using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public abstract class ComparisonConditionBase : WritableBase, ICondition
{
    public IConditionKeyword Left { get; }
    public IConditionKeyword Right { get; }
    public ComparisonConditionBase(IConditionKeyword left, IConditionKeyword right)
    {
        Left = left;
        Right = right;
    }
    public abstract bool Evaluate(IVariableDictionary variables);
}
