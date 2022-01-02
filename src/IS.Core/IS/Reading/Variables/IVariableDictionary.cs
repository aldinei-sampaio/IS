namespace IS.Reading.Variables;

public interface IVariableDictionary
{
    object? this[string name] { get; set; }
    int Count { get; }
    bool IsSet(string name);
}
