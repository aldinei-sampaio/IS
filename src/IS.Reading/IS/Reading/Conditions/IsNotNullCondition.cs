using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class IsNotNullCondition : ICondition
{
    public IConditionKeyword Operand { get; }
    public IsNotNullCondition(IConditionKeyword operand) => Operand = operand;
    public bool Evaluate(IVariableDictionary variables) => Operand.Evaluate(variables) is not null;

    public void WriteTo(TextWriter textWriter)
    {
        Operand.WriteTo(textWriter);
        textWriter.Write(" Is Not Null");
    }
}
