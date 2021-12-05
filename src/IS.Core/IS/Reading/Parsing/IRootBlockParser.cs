using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing
{
    public interface IRootBlockParser
    {
        IElementParserSettings Settings { get; }

        Task<IBlock> ParseAsync(XmlReader reader, IParsingContext parsingContext);
    }
}