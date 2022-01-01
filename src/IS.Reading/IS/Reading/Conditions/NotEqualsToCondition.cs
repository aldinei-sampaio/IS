﻿using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class NotEqualsToCondition : ComparisonCondition
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

        return leftValue.CompareTo(rightValue) != 0;
    }
}
