using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class VariableCondition : IConditionKeyword
{
    public string Name { get; }
    public VariableCondition(string name) => Name = name;

    public object? Evaluate(IVariableDictionary variables)
    {
        if (variables.Integers.IsSet(Name))
            return variables.Integers[Name];

        if (variables.Strings.IsSet(Name))
            return variables.Strings[Name];

        return null;
    }
}
