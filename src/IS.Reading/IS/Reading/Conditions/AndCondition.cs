using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class AndCondition : ICondition
{
    public ICondition Left { get; }
    public ICondition Right { get; }
    public AndCondition(ICondition left, ICondition right)
    {
        Left = left;
        Right = right;
    }

    public bool Evaluate(IVariableDictionary variables)
        => Left.Evaluate(variables) && Right.Evaluate(variables);
}
