using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public interface IConditionKeyword
{
    abstract object? Evaluate(IVariableDictionary variables);
}
