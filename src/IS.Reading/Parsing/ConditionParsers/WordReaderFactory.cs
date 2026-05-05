namespace IS.Reading.Parsing.ConditionParsers;

public interface IWordReaderFactory
{
    IWordReader Create(string text);
}

public class WordReaderFactory : IWordReaderFactory
{
    public IWordReader Create(string text)
        => new WordReader(text);
}
