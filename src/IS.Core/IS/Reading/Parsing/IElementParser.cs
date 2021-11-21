using System.Xml;

namespace IS.Reading.Parsing;

public interface IElementParser
{
    ParserDictionary<IAttributeParser> AttributeParsers { get; }
    ParserDictionary<INodeParser> ChildParsers { get; }
    ITextParser? TextParser { get; set; }
    Task<IElementParsedData?> ParseAsync(XmlReader reader, IParsingContext parsingContext);
}
