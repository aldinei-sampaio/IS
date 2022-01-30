using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing
{
    public interface IRootBlockParser
    {
        IElementParserSettings Settings { get; }

        Task<List<INode>> ParseAsync(IDocumentReader reader, IParsingContext parsingContext);
    }
}