using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class InCondition(IConditionKeyword operand, IEnumerable<IConditionKeyword> values) : WritableBase, ICondition
{
    public IConditionKeyword Operand { get; } = operand;
    public IEnumerable<IConditionKeyword> Values { get; } = values;

    public bool Evaluate(IVariableDictionary variables)
    {
        var actual = Operand.Evaluate(variables);

        foreach (var expected in Values)
            if (Helper.AreEqual(expected.Evaluate(variables), actual))
                return true;

        return false;
    }

    public override void WriteTo(TextWriter textWriter)
    {
        Operand.WriteTo(textWriter);
        textWriter.Write(" In (");
        Helper.WriteValues(textWriter, Values);
        textWriter.Write(")");
    }
}
