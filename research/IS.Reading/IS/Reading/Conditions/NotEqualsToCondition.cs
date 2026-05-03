using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class NotEqualsToCondition : ComparisonConditionBase
{
    public NotEqualsToCondition(IConditionKeyword left, IConditionKeyword right) : base(left, right)
    {
    }

    public override bool Evaluate(IVariableDictionary variables)
    {
        var rightValue = Right.Evaluate(variables) as IComparable;

        if (Left.Evaluate(variables) is not IComparable leftValue)
            return rightValue is not null;

        if (rightValue is null)
            return true;

        return leftValue.GetType() == rightValue.GetType() && leftValue.CompareTo(rightValue) != 0;
    }

    public override void WriteTo(TextWriter textWriter)
    {
        Left.WriteTo(textWriter);
        textWriter.Write(" != ");
        Right.WriteTo(textWriter);
    }
}
