using IS.Reading.Parsing.Attributes;
using IS.Reading.Parsing.Text;
using System.Xml;

namespace IS.Reading.Parsing.Nodes
{
    public interface IElementParser
    {
        ParserDictionary<IAttributeParser> AttributeParsers { get; }
        ParserDictionary<INodeParser> ChildParsers { get; }
        ITextParser? TextParser { get; set; }

        Task<ElementParsedData?> ParseAsync(XmlReader reader, IParsingContext parsingContext);
    }
}