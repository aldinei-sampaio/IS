namespace IS.Reading.Variables;

public struct VariableTextSource : ITextSource
{
    public VariableTextSource(string variableName)
        => VariableName = variableName;

    public string VariableName { get; }

    public string Build(IVariableDictionary variables)
        => variables[VariableName] as string ?? string.Empty;

    public override string ToString()
        => $"{{{VariableName}}}";
}