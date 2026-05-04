using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class NotInCondition(IConditionKeyword operand, IEnumerable<IConditionKeyword> values) : WritableBase, ICondition
{
    public IConditionKeyword Operand { get; } = operand;
    public IEnumerable<IConditionKeyword> Values { get; } = values;

    public bool Evaluate(IVariableDictionary variables)
    {
        var actual = Operand.Evaluate(variables);

        foreach (var value in Values)
            if (Helper.AreEqual(value.Evaluate(variables), actual))
                return false;

        return true;
    }

    public override void WriteTo(TextWriter textWriter)
    {
        Operand.WriteTo(textWriter);
        textWriter.Write(" Not In (");
        Helper.WriteValues(textWriter, Values);
        textWriter.Write(")");
    }
}
