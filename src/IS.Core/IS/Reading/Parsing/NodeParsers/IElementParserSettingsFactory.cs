namespace IS.Reading.Parsing.NodeParsers;

public interface IElementParserSettingsFactory
{
    IElementParserSettings IfBlock { get; }
    IElementParserSettings Block { get; }
    IElementParserSettings NoBlock { get; }
}