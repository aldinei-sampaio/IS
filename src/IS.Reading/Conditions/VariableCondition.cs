using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class VariableCondition(string name) : WritableBase, IConditionKeyword
{
    public string Name { get; } = name;

    public object? Evaluate(IVariableDictionary variables) => variables[Name];

    public override void WriteTo(TextWriter textWriter) => textWriter.Write(Name);
}
