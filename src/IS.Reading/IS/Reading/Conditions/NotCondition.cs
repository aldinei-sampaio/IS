using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class NotCondition : WritableBase, ICondition
{
    public ICondition Condition { get; }
    public NotCondition(ICondition condition) => Condition = condition;

    public bool Evaluate(IVariableDictionary variables)
        => !Condition.Evaluate(variables);

    public override void WriteTo(TextWriter textWriter)
    {
        textWriter.Write("Not (");
        Condition.WriteTo(textWriter);
        textWriter.Write(")");
    }
}
