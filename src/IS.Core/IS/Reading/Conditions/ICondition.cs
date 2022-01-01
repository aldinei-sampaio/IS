using IS.Reading.Variables;

namespace IS.Reading.Conditions
{
    public interface ICondition
    {
        bool Evaluate(IVariableDictionary variables);
    }
}
