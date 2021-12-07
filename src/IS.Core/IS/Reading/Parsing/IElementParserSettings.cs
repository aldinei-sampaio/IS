namespace IS.Reading.Parsing;

public interface IElementParserSettings
{
    ITextParser? TextParser { get; }
    IParserDictionary<IAttributeParser> AttributeParsers { get; }
    IParserDictionary<INodeParser> ChildParsers { get; }
    bool ExitOnUnknownNode { get; }

    bool NoRepeatNode { get; }
}
