namespace IS.Reading.Parsing.ConditionParsers;

public class WordReaderFactory : IWordReaderFactory
{
    public IWordReader Create(string text)
        => new WordReader(text);
}
