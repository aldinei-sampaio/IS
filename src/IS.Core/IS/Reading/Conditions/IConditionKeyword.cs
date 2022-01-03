using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public interface IConditionKeyword : IWritable
{
    object? Evaluate(IVariableDictionary variables);
}
