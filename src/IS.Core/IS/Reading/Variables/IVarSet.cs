namespace IS.Reading.Variables;

public interface IVarSet
{
    string Name { get; }
    IVarSet Execute(IVariableDictionary variables);
}
