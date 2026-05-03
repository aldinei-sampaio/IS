using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class VariableCondition : WritableBase, IConditionKeyword
{
    public string Name { get; }
    public VariableCondition(string name) => Name = name;
    public object? Evaluate(IVariableDictionary variables) => variables[Name];
    public override void WriteTo(TextWriter textWriter) => textWriter.Write(Name);
}
