namespace IS.Reading.Variables;

public interface IIntegerDictionary
{
    int? this[string name] { get; set; }
    int Count { get; }
    bool IsSet(string name);
}
