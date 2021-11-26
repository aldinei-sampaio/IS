using IS.Reading.State;

namespace IS.Reading.Navigation
{
    public interface ICondition
    {
        bool Evaluate(IVariableDictionary variables);
    }
}
