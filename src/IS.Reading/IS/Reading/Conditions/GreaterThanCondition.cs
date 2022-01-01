using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class GreaterThanCondition : ComparisonCondition
{
    public GreaterThanCondition(IConditionKeyword left, IConditionKeyword right) : base(left, right)
    {
    }

    public override bool Evaluate(IVariableDictionary variables)
    {
        if (Left.Evaluate(variables) is not IComparable leftValue)
            return false;

        if (Right.Evaluate(variables) is not IComparable rightValue)
            return true;

        return leftValue.CompareTo(rightValue) > 0;
    }
}
