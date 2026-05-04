using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class IsNullCondition(IConditionKeyword operand) : WritableBase, ICondition
{
    public IConditionKeyword Operand { get; } = operand;

    public bool Evaluate(IVariableDictionary variables) => Operand.Evaluate(variables) is null;

    public override void WriteTo(TextWriter textWriter)
    {
        Operand.WriteTo(textWriter);
        textWriter.Write(" Is Null");
    }
}
