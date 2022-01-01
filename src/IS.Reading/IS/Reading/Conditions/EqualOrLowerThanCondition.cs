using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class EqualOrLowerThanCondition : ComparisonCondition
{
    public EqualOrLowerThanCondition(IConditionKeyword left, IConditionKeyword right) : base(left, right)
    {
    }

    public override bool Evaluate(IVariableDictionary variables)
    {
        if (Right.Evaluate(variables) is not IComparable rightValue)
            return true;

        if (Left.Evaluate(variables) is not IComparable leftValue)
            return false;

        return leftValue.CompareTo(rightValue) <= 0;
    }
}
