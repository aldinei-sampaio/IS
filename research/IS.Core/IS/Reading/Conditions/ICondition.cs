using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public interface ICondition : IWritable
{
    bool Evaluate(IVariableDictionary variables);
}
