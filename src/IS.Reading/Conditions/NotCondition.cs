using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class NotCondition(ICondition condition) : WritableBase, ICondition
{
    public ICondition Condition { get; } = condition;

    public bool Evaluate(IVariableDictionary variables)
        => !Condition.Evaluate(variables);

    public override void WriteTo(TextWriter textWriter)
    {
        textWriter.Write("Not (");
        Condition.WriteTo(textWriter);
        textWriter.Write(")");
    }
}
