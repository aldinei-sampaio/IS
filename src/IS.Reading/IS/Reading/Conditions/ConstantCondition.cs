using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class ConstantCondition : IConditionKeyword
{
    public object Value { get; }
    public ConstantCondition(object value) => Value = value;
    public object? Evaluate(IVariableDictionary variables) => Value;    
}
