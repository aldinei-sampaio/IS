using IS.Reading.Navigation;

namespace IS.Reading.Parsing
{
    public interface IRootBlockParser
    {
        Task<List<INode>> ParseAsync(IDocumentReader reader, IParsingContext parsingContext);
    }
}