using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class EqualOrGreaterThanCondition : ComparisonCondition
{
    public EqualOrGreaterThanCondition(IConditionKeyword left, IConditionKeyword right) : base(left, right)
    {
    }

    public override bool Evaluate(IVariableDictionary variables)
    {
        if (Left.Evaluate(variables) is not IComparable leftValue)
            return true;

        if (Right.Evaluate(variables) is not IComparable rightValue)
            return false;

        return leftValue.CompareTo(rightValue) >= 0;
    }

    public override void WriteTo(TextWriter textWriter)
    {
        Left.WriteTo(textWriter);
        textWriter.Write(" >= ");
        Right.WriteTo(textWriter);
    }
}
