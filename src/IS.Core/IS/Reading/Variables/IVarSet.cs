namespace IS.Reading.Variables;

public interface IVarSet
{
    string Name { get; }
    object? Execute(IVariableDictionary variables);
}
