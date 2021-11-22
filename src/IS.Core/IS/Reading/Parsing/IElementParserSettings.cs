namespace IS.Reading.Parsing;

public interface IElementParserSettings
{
    ITextParser? TextParser { get; set; }
    ParserDictionary<IAttributeParser> AttributeParsers { get; }
    ParserDictionary<INodeParser> ChildParsers { get; }
}
