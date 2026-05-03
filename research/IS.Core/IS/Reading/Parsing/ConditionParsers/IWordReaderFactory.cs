
namespace IS.Reading.Parsing.ConditionParsers
{
    public interface IWordReaderFactory
    {
        IWordReader Create(string text);
    }
}