using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class IsNullCondition : WritableBase, ICondition
{
    public IConditionKeyword Operand { get; }
    public IsNullCondition(IConditionKeyword operand) => Operand = operand;
    public bool Evaluate(IVariableDictionary variables) => Operand.Evaluate(variables) is null;

    public override void WriteTo(TextWriter textWriter)
    {
        Operand.WriteTo(textWriter);
        textWriter.Write(" Is Null");
    }
}
