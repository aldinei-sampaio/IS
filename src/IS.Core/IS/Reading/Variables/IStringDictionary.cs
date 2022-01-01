namespace IS.Reading.Variables;

public interface IStringDictionary
{
    string? this[string name] { get; set; }
    int Count { get; }
    bool IsSet(string name);
}
