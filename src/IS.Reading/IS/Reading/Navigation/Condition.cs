using IS.Reading.Conditions;
using IS.Reading.Variables;

namespace IS.Reading.Navigation;

public class Condition : ICondition
{
    public string[] VariableNames { get; }
    public ConditionType Operator { get; }
    public int Value { get; }
    public int Value2 { get; }

    public Condition(string[] names, ConditionType op, int value, int value2)
    {
        VariableNames = names;
        Operator = op;
        Value = value;
        Value2 = value2;
    }

    public bool EvaluateFor(int value)
    {
        return Operator switch
        {
            ConditionType.Defined => value > 0,
            ConditionType.Undefined => value <= 0,
            ConditionType.NotEqualTo => value != Value,
            ConditionType.EqualOrLessThan => value <= Value,
            ConditionType.LessThan => value < Value,
            ConditionType.EqualOrGreaterThan => value >= Value,
            ConditionType.GreaterThan => value > Value,
            ConditionType.Between => value >= Value && value <= Value2,
            ConditionType.NotBetween => value < Value || value > Value2,
            _ => value == Value,
        };
    }

    public bool EvaluateFor(string? value)
    {
        return Operator switch
        {
            ConditionType.Defined => value is not null,
            ConditionType.Undefined => value is null,
            _ => false
        };
    }

    public override string ToString()
    {
        var joinedNames = string.Join(",", VariableNames);
        return Operator switch
        {
            ConditionType.Defined => $"{joinedNames}[1:]",
            ConditionType.Undefined => $"{joinedNames}[:0]",
            ConditionType.NotEqualTo => $"!{joinedNames}[{Value}]",
            ConditionType.EqualOrLessThan => $"{joinedNames}[:{Value}]",
            ConditionType.LessThan => $"!{joinedNames}[{Value}:]",
            ConditionType.EqualOrGreaterThan => $"{joinedNames}[{Value}:]",
            ConditionType.GreaterThan => $"!{joinedNames}[:{Value}]",
            ConditionType.Between => $"{joinedNames}[{Value}:{Value2}]",
            ConditionType.NotBetween => $"!{joinedNames}[{Value}:{Value2}]",
            _ => $"{joinedNames}[{Value}]",
        };
    }

    private bool EvaluateFor(IVariableDictionary variables, string name)
    {
        var intValue = variables.Integers[name];
        if (intValue.HasValue)
            return EvaluateFor(intValue.Value);
        return EvaluateFor(variables.Strings[name]);
    }

    public bool Evaluate(IVariableDictionary variables)
    {
        var mustHaveDefined = Operator == ConditionType.Defined;

        if (VariableNames.Length == 1)
        {
            if (mustHaveDefined)
                return variables.IsSet(VariableNames[0]);

            if (Operator == ConditionType.Undefined)
                return !variables.IsSet(VariableNames[0]);

            return EvaluateFor(variables, VariableNames[0]);
        }

        foreach (var name in VariableNames)
        {
            if (variables.IsSet(name))
                return mustHaveDefined;
        }

        return !mustHaveDefined;
    }
}
