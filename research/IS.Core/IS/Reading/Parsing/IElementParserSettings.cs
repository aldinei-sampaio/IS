namespace IS.Reading.Parsing;

public interface IElementParserSettings
{
    IParserDictionary<INodeParser> ChildParsers { get; }
    bool ExitOnUnknownNode { get; }
    bool ExitOnEnd { get; }
    bool ExitOnElse { get; }
    bool NoRepeatNode { get; }
    bool ParseCurrent { get; }
}
