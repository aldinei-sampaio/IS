using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class OrCondition(ICondition left, ICondition right) : WritableBase, ICondition
{
    public ICondition Left { get; } = left;
    public ICondition Right { get; } = right;

    public bool Evaluate(IVariableDictionary variables)
        => Left.Evaluate(variables) || Right.Evaluate(variables);

    public override void WriteTo(TextWriter textWriter)
    {
        textWriter.Write("(");
        Left.WriteTo(textWriter);
        textWriter.Write(") Or (");
        Right.WriteTo(textWriter);
        textWriter.Write(")");
    }
}
