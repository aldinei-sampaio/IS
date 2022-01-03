using IS.Reading.Variables;

namespace IS.Reading.Conditions;

internal class FakeToStringCondition : ICondition, IConditionKeyword
{
    private readonly string expression;

    public FakeToStringCondition(string expression)
        => this.expression = expression;

    public bool Evaluate(IVariableDictionary variables)    
        => throw new NotImplementedException();

    public void WriteTo(TextWriter textWriter)
        => textWriter.Write(expression);

    object IConditionKeyword.Evaluate(IVariableDictionary variables)
        => throw new NotImplementedException();
}