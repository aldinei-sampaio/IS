using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class InCondition : ICondition
{
    public IConditionKeyword Operand { get; }
    public IEnumerable<IConditionKeyword> Values { get; }
    public InCondition(IConditionKeyword operand, IEnumerable<IConditionKeyword> values)
    {
        Operand = operand;
        Values = values;
    }
    public bool Evaluate(IVariableDictionary variables)
    {
        var actual = Operand.Evaluate(variables);

        foreach (var expected in Values)
            if (Helper.AreEqual(expected, actual))
                return true;
        
        return false;
    }

    public void WriteTo(TextWriter textWriter)
    {
        Operand.WriteTo(textWriter);
        textWriter.Write(" In (");
        Helper.WriteValues(textWriter, Values);
        textWriter.Write(")");
    }
}
