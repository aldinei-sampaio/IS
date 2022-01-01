using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public abstract class ComparisonCondition : ICondition
{
    public IConditionKeyword Left { get; }
    public IConditionKeyword Right { get; }
    public ComparisonCondition(IConditionKeyword left, IConditionKeyword right)
    {
        Left = left;
        Right = right;
    }
    public abstract bool Evaluate(IVariableDictionary variables);
}
