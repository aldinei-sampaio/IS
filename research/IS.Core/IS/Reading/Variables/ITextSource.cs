namespace IS.Reading.Variables;

public interface ITextSource
{
    string Build(IVariableDictionary variables);
}
