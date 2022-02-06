namespace IS.Reading.Parsing;

public interface IElementParserSettings
{
    IParserDictionary<INodeParser> ChildParsers { get; }
    bool ExitOnUnknownNode { get; }
    bool IsBlock { get; }
    bool ExitOnElse { get; }
    bool NoRepeatNode { get; }
}
