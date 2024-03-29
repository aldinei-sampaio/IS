﻿using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class NotBetweenCondition : WritableBase, ICondition
{
    public IConditionKeyword Operand { get; }
    public IConditionKeyword Min { get; }
    public IConditionKeyword Max { get; }
    public NotBetweenCondition(IConditionKeyword operand, IConditionKeyword min, IConditionKeyword max)
    {
        Operand = operand;
        Min = min;
        Max = max;
    }
    public bool Evaluate(IVariableDictionary variables)
    {
        if (Max.Evaluate(variables) is not IComparable maxValue)
            return true;

        var minValue = Min.Evaluate(variables) as IComparable;

        if (Operand.Evaluate(variables) is not IComparable actual)
            return minValue is not null;

        if (minValue is not null)
        {
            if (actual.GetType() != minValue.GetType())
                return false;

            if (actual.CompareTo(minValue) < 0)
                return true;
        }

        return actual.GetType() == maxValue.GetType() && actual.CompareTo(maxValue) > 0;
    }

    public override void WriteTo(TextWriter textWriter)
    {
        Operand.WriteTo(textWriter);
        textWriter.Write(" Not Between ");
        Min.WriteTo(textWriter);
        textWriter.Write(" And ");
        Max.WriteTo(textWriter);
    }
}
