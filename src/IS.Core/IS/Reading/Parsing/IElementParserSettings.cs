namespace IS.Reading.Parsing;

public interface IElementParserSettings
{
    ITextParser? TextParser { get; set; }
    IParserDictionary<IAttributeParser> AttributeParsers { get; }
    IParserDictionary<INodeParser> ChildParsers { get; }
}
