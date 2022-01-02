using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public interface IConditionKeyword : IWritable
{
    abstract object? Evaluate(IVariableDictionary variables);
}
