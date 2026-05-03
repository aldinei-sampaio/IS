namespace IS.Reading.Parsing.ConditionParsers
{
    public interface IWordReader
    {
        bool AtEnd { get; }
        string Word { get; }
        WordType WordType { get; }
        bool Read();
    }
}