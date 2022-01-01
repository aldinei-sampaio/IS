namespace IS.Reading.Variables;

public interface IVariableDictionary
{
    IStringDictionary Strings { get; }
    IIntegerDictionary Integers { get; }
    int Count { get; }
    void Unset(string name);
    bool IsSet(string name);
}
