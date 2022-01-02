using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class BetweenCondition : ICondition
{
    public IConditionKeyword Operand { get; }
    public IConditionKeyword Min { get; }
    public IConditionKeyword Max { get; }
    public BetweenCondition(IConditionKeyword operand, IConditionKeyword min, IConditionKeyword max)
    {
        Operand = operand;
        Min = min;
        Max = max;
    }
    public bool Evaluate(IVariableDictionary variables)
    {
        if (Max.Evaluate(variables) is not IComparable maxValue)
            return false;

        var minValue = Min.Evaluate(variables) as IComparable;

        if (Operand.Evaluate(variables) is not IComparable actual)
            return minValue is null;

        if (minValue is not null)
        {
            if (actual.CompareTo(minValue) < 0)
                return false;
        }

        return actual.CompareTo(maxValue) <= 0;
    }

    public void WriteTo(TextWriter textWriter)
    {
        Operand.WriteTo(textWriter);
        textWriter.Write(" Between ");
        Min.WriteTo(textWriter);
        textWriter.Write(" And ");
        Max.WriteTo(textWriter);
    }
}
